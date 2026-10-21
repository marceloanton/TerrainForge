from __future__ import annotations

import re
import shutil
import subprocess
import tempfile
from pathlib import Path

import markdown

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "docs" / "manual"
WIKI_URL = "https://github.com/marceloanton/TerrainForge.wiki.git"
LANGUAGES = {
    "Home": ("English", "en"),
    "Home-es": ("Español", "es"),
    "Home-fr": ("Français", "fr"),
    "Home-de": ("Deutsch", "de"),
    "Home-ru": ("Русский", "ru"),
    "Home-zh": ("简体中文", "zh"),
    "Home-zh-tw": ("繁體中文", "zh-Hant"),
    "Home-ja": ("日本語", "ja"),
    "Home-ko": ("한국어", "ko"),
    "Home-pl": ("Polski", "pl"),
    "Home-pt": ("Português", "pt"),
    "Home-tr": ("Türkçe", "tr"),
    "Home-uk": ("Українська", "uk"),
    "Home-it": ("Italiano", "it"),
}
EXTENSIONS = ["tables", "fenced_code", "toc", "sane_lists", "attr_list", "footnotes"]


def run(*args: str) -> None:
    subprocess.run(args, check=True, cwd=ROOT)


def localize_internal_links(html: str, page_stems: set[str]) -> str:
    def replace(match: re.Match[str]) -> str:
        url = match.group(1)
        if url.startswith(("https://", "http://", "mailto:", "#", "/")):
            return match.group(0)
        path, separator, suffix = url.partition("#")
        if not path or Path(path).suffix:
            return match.group(0)
        stem = Path(path).stem
        if stem not in page_stems:
            return match.group(0)
        updated = f"{path}.html"
        if separator:
            updated += f"#{suffix}"
        return f'href="{updated}"'

    return re.sub(r'href="([^"]+)"', replace, html)


def wrap_page(stem: str, title: str, lang: str, body: str, page_stems: set[str]) -> str:
    body = localize_internal_links(body, page_stems)
    links = " · ".join(
        f'<a href="{page}.html" lang="{code}">{label}</a>'
        for page, (label, code) in LANGUAGES.items()
    )
    return f'''<!doctype html>
<html lang="{lang}">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="theme-color" content="#111713">
  <title>{title} · TerrainForge</title>
  <link rel="icon" href="../icon.png" type="image/png">
  <link rel="stylesheet" href="../style.css">
  <style>
    .wiki-shell{{max-width:1100px;margin:0 auto;padding:34px clamp(20px,5vw,64px) 80px}}
    .wiki-top{{border-bottom:1px solid rgba(231,224,202,.16);padding-bottom:18px;margin-bottom:38px}}
    .wiki-top>a{{color:#ffc36d;font:600 13px 'DM Mono',monospace}}
    .wiki-languages{{margin-top:18px;color:#b9c2ba;font-size:13px;line-height:2.2}}
    .wiki-languages a{{color:#e7c37e;margin-right:7px;white-space:nowrap}}
    .wiki-content{{color:#e8e5d9;font:16px/1.75 'DM Sans',Arial,sans-serif}}
    .wiki-content h1,.wiki-content h2,.wiki-content h3,.wiki-content h4{{color:#ffc36d;font-family:'Averia Serif Libre',Georgia,serif;line-height:1.25;margin:1.8em 0 .65em}}
    .wiki-content h1{{font-size:clamp(32px,5vw,48px);margin-top:0}}
    .wiki-content h2{{font-size:30px;border-bottom:1px solid rgba(231,224,202,.16);padding-bottom:9px}}
    .wiki-content h3{{font-size:23px}}
    .wiki-content a{{color:#55dce9;text-decoration:underline;text-underline-offset:3px}}
    .wiki-content img{{max-width:100%;height:auto}}
    .wiki-content table{{display:block;max-width:100%;overflow:auto;border-collapse:collapse}}
    .wiki-content th,.wiki-content td{{border:1px solid rgba(231,224,202,.2);padding:9px 12px;text-align:left}}
    .wiki-content th{{background:#1b2c31;color:#ffc36d}}
    .wiki-content blockquote{{margin:1.4em 0;padding:5px 18px;border-left:3px solid #cf7634;background:#14232a;color:#c9d0c7}}
    .wiki-content pre{{overflow:auto;padding:18px;background:#091116;border:1px solid rgba(231,224,202,.16);border-radius:5px}}
    .wiki-content code{{font-family:'DM Mono',monospace;font-size:.9em}}
    .wiki-content li{{margin:.3em 0}}
    @media(max-width:600px){{.wiki-content{{font-size:15px}}.wiki-content h2{{font-size:25px}}}}
  </style>
</head>
<body>
  <header class="wiki-shell wiki-top">
    <a href="../index.html">← TerrainForge</a>
    <nav class="wiki-languages" aria-label="Manual languages">{links}</nav>
  </header>
  <main class="wiki-shell wiki-content">{body}</main>
  <footer><a class="footer-brand" href="../index.html">TERRAINFORGE</a><span>Documentation synced from the GitHub Wiki.</span><a href="https://github.com/marceloanton/TerrainForge/wiki">GITHUB WIKI ↗</a></footer>
</body>
</html>
'''


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    for old_page in OUTPUT.glob("*.html"):
        old_page.unlink()
    with tempfile.TemporaryDirectory(prefix="terrainforge-wiki-") as temp:
        wiki = Path(temp) / "wiki"
        run("git", "clone", "--depth", "1", WIKI_URL, str(wiki))
        pages = sorted(path for path in wiki.glob("*.md") if path.name != "_Sidebar.md")
        if not pages:
            raise RuntimeError("No Markdown pages were found in the GitHub Wiki.")
        stems = {path.stem for path in pages}
        for source in pages:
            stem = source.stem
            label, lang = LANGUAGES.get(stem, (stem.replace("-", " "), "en"))
            text = source.read_text(encoding="utf-8")
            body = markdown.markdown(text, extensions=EXTENSIONS, output_format="html5")
            page = wrap_page(stem, label, lang, body, stems)
            (OUTPUT / f"{stem}.html").write_text(page, encoding="utf-8", newline="\n")
        images = wiki / "images"
        target_images = ROOT / "docs" / "images"
        if images.is_dir():
            target_images.mkdir(parents=True, exist_ok=True)
            shutil.copytree(images, target_images, dirs_exist_ok=True)
        spanish = "Home-es.html" if "Home-es" in stems else "Home.html"
        (OUTPUT / "index.html").write_text(
            f'<!doctype html><meta charset="utf-8"><meta http-equiv="refresh" content="0;url={spanish}"><a href="{spanish}">Open TerrainForge manual</a>\n',
            encoding="utf-8",
            newline="\n",
        )
        print(f"Rendered {len(pages)} wiki pages to {OUTPUT}")


if __name__ == "__main__":
    main()

