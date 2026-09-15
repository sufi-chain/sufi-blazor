import { defineConfig } from "vite";
import { resolve } from "node:path";

export default defineConfig({
  resolve: {
    alias: {
      mermaid: resolve(__dirname, "node_modules/mermaid/dist/mermaid.min.js"),
    },
  },
  build: {
    lib: {
      entry: {
        "rich-text": resolve(__dirname, "src/rich-text/index.ts"),
        code: resolve(__dirname, "src/code/index.ts"),
        diff: resolve(__dirname, "src/diff/index.ts"),
        viewer: resolve(__dirname, "src/viewer/index.ts"),
      },
      formats: ["es"],
    },
    outDir: resolve(__dirname, "../src/SufiChain.SufiBlazor/wwwroot/_sufi/editors"),
    emptyOutDir: true,
    sourcemap: false,
    minify: true,
    cssCodeSplit: true,
    rollupOptions: {
      output: {
        entryFileNames: "[name]/index.js",
        chunkFileNames: "chunks/[name]-[hash].js",
        assetFileNames: (asset) => {
          const names = [...(asset.names ?? []), asset.name].filter(Boolean);
          if (names.some((name) => String(name).endsWith(".css"))) {
            return "styles/[name][extname]";
          }
          return "assets/[name][extname]";
        },
      },
    },
  },
});
