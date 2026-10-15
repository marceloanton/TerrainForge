Add-Type -AssemblyName System.Drawing
$src = 'C:\Users\Marce\Downloads\TerrainForge.png'
$dst = 'C:\Users\Marce\OneDrive\Escritorio\htdocs\Proyectos\MODS\Valheim\Terrain\icon.png'
$img = [System.Drawing.Image]::FromFile($src)
$thumb = $img.GetThumbnailImage(256, 256, $null, [IntPtr]::Zero)
$thumb.Save($dst, [System.Drawing.Imaging.ImageFormat]::Png)
$img.Dispose()
$thumb.Dispose()
Write-Host 'icon.png creado (256x256)'