# Wiki sources

This folder contains the Markdown sources for the **GitHub Wiki**. These files are not shipped inside the Thunderstore mod zip.

## Pages

- `Home.md` — English manual and default wiki page.
- `Overview.md` — English overview based on the project README.
- `Home-<code>.md` — translated manuals (13 languages).
- `Roadmap.md` — project roadmap.
- `Changelog.md` — English release history.
- `_Sidebar.md` — shared language selector and English table of contents.

## Publish the wiki

GitHub creates the separate `TerrainForge.wiki` repository only after the first wiki page is created. Do this once in the browser:

1. Open [TerrainForge on GitHub](https://github.com/marceloanton/TerrainForge).
2. Go to **Settings → General → Features** and enable **Wikis**.
3. Open the **Wiki** tab, choose **Create the first page**, name it `Home`, paste the contents of `Home.md`, and save.

Then publish the rest from PowerShell, running these commands from the project folder:

```powershell
git clone https://github.com/marceloanton/TerrainForge.wiki.git
Copy-Item .\wiki\Home*.md, .\wiki\Overview.md, .\wiki\Roadmap.md, .\wiki\Changelog.md, .\wiki\_Sidebar.md .\TerrainForge.wiki\ -Force
Set-Location .\TerrainForge.wiki
git add Home*.md Overview.md Roadmap.md Changelog.md _Sidebar.md
git commit -m "docs: publish TerrainForge wiki"
git push
```

The language links in the pages use GitHub Wiki page names (`Home-es`, `Home-fr`, and so on). The matching pages must be included in the push for those links to work.
