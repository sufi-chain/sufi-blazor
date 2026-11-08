using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Data;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

file class FilterBarStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        Array.Empty<LocalizedString>();
}

file sealed class ManualTimeProvider : TimeProvider
{
    private readonly object _gate = new();
    private readonly List<ManualTimer> _timers = new();

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = new ManualTimer(this, callback, state, dueTime, period);
        lock (_gate)
        {
            _timers.Add(timer);
        }

        if (dueTime == TimeSpan.Zero)
        {
            Fire(timer);
        }

        return timer;
    }

    public void Advance(TimeSpan by)
    {
        if (by < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(by));
        }

        List<ManualTimer> due;
        lock (_gate)
        {
            due = _timers
                .Where(timer => timer.Armed && timer.Due <= by)
                .OrderBy(timer => timer.Due)
                .ToList();
            foreach (var timer in _timers)
            {
                if (!due.Contains(timer) && timer.Armed)
                {
                    timer.Due -= by;
                }
            }

            foreach (var timer in due)
            {
                _timers.Remove(timer);
            }
        }

        foreach (var timer in due)
        {
            timer.Invoke();
        }
    }

    internal void Remove(ManualTimer timer)
    {
        lock (_gate)
        {
            _timers.Remove(timer);
        }
    }

    private void Fire(ManualTimer timer)
    {
        lock (_gate)
        {
            _timers.Remove(timer);
        }

        timer.Invoke();
    }

    internal sealed class ManualTimer : ITimer
    {
        private readonly ManualTimeProvider _provider;
        private readonly TimerCallback _callback;
        private readonly object? _state;
        private int _disposed;

        public ManualTimer(
            ManualTimeProvider provider,
            TimerCallback callback,
            object? state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            _provider = provider;
            _callback = callback;
            _state = state;
            Due = dueTime;
            Period = period;
            Armed = dueTime != Timeout.InfiniteTimeSpan;
        }

        public TimeSpan Due { get; set; }

        public TimeSpan Period { get; private set; }

        public bool Armed { get; private set; }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            Due = dueTime;
            Period = period;
            Armed = dueTime != Timeout.InfiniteTimeSpan;
            return true;
        }

        public void Invoke() => _callback(_state);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                Armed = false;
                _provider.Remove(this);
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

public class SbFilterBarTests : BunitContext
{
    public SbFilterBarTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new FilterBarStringLocalizer());
    }

    [Fact]
    public async Task RapidInputReportsCompleteBrowserValueAfterDebounce()
    {
        var clock = new ManualTimeProvider();
        var reportedValue = new TaskCompletionSource<string?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = Render<SbFilterBar>(parameters => parameters
            .Add(p => p.SearchDebounceMs, 50)
            .Add(p => p.Clock, clock)
            .Add(p => p.SearchValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValue.TrySetResult(value))));

        var input = cut.Find(".sb-filter-bar__search-input");
        await cut.InvokeAsync(() =>
        {
            input.Input("a");
            input.Input("ar");
            input.Input("ari");
            input.Input("aria");
            Assert.False(reportedValue.Task.IsCompleted);
            clock.Advance(TimeSpan.FromMilliseconds(49));
            Assert.False(reportedValue.Task.IsCompleted);
            clock.Advance(TimeSpan.FromMilliseconds(1));
        });

        Assert.Equal(
            "aria",
            await reportedValue.Task.WaitAsync(TimeSpan.FromSeconds(5)));
    }
}
