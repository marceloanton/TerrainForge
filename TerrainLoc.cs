using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TerrainForge
{
    /// <summary>
    /// EN/ES strings for every user-visible text. Key -> [es, en].
    /// </summary>
    internal static partial class Loc
    {
        private static readonly System.Collections.Generic.Dictionary<string, string[]> s_texts =
            new System.Collections.Generic.Dictionary<string, string[]>
            {
                // Tabs + generic buttons
                { "tab.keys", new[] { "ATAJOS", "CONTROLS" } },
                { "tab.help", new[] { "GUÍA", "GUIDE" } },
                { "tab.settings", new[] { "OPCIONES", "OPTIONS" } },
                { "tab.tool", new[] { "BROCHA", "BRUSH" } },
                { "tab.advanced", new[] { "GUARDADOS", "SAVED" } },
                { "tab.stamps", new[] { "COPIAR/PEGAR", "COPY/PASTE" } },
                { "tabintro.tool", new[] { "Define qué hace la brocha y cómo afecta el terreno.", "Choose the brush operation, shape, and terrain effect." } },
                { "tabintro.keys", new[] { "Cambia los atajos: selecciona una acción y pulsa la nueva tecla.", "Change shortcuts: select an action, then press its new key." } },
                { "tabintro.advanced", new[] { "Guarda y recupera perfiles, stamps y copias del terreno.", "Save and restore brush profiles, stamps, and terrain backups." } },
                { "tabintro.settings", new[] { "Ajusta la herramienta, la vista y las reglas del servidor.", "Configure the tool, display, and server rules." } },
                { "tabintro.stamps", new[] { "Copia terreno, coloca una vista previa y pega el resultado. También puedes guardar y cargar stamps.", "Copy terrain, position a preview, and paste it. You can also save and load stamps." } },
                { "tabintro.help", new[] { "Guía rápida para usar TerrainForge.", "A quick guide to using TerrainForge." } },
                { "set.search", new[] { "Buscar", "Search" } },
                { "btn.close", new[] { "Cerrar", "Close" } },
                { "btn.get", new[] { "Obtener la herramienta", "Get the tool" } },
                { "menu.coffee", new[] { "Apoyar TerrainForge en Cafecito", "Support TerrainForge on Cafecito" } },
                { "keys.hint.idle", new[] { "Haz click en una tecla y luego pulsa la nueva.", "Click a key, then press the new one." } },
                { "keys.hint.capture", new[] { "Pulsa una tecla o boton del raton...   (Esc cancela)", "Press a key or mouse button...   (Esc cancels)" } },
                { "keys.press", new[] { "< pulsa una tecla... >", "< press a key... >" } },
                // Key labels
                { "key.apply", new[] { "Aplicar (mantener)", "Apply (hold)" } },
                { "key.radiusUp", new[] { "Subir radio", "Radius up" } },
                { "key.radiusDown", new[] { "Bajar radio", "Radius down" } },
                { "key.cycleOp", new[] { "Cambiar operacion", "Cycle operation" } },
                { "key.cycleShape", new[] { "Cambiar forma", "Cycle shape" } },
                { "key.cyclePaint", new[] { "Cambiar pintura", "Cycle paint" } },
                { "key.aimMod", new[] { "Nivelar a tus pies (mod.)", "Level to feet (mod.)" } },
                { "key.undo", new[] { "Deshacer", "Undo" } },
                { "key.redo", new[] { "Rehacer", "Redo" } },
                { "key.eyedrop", new[] { "Fijar altura (eyedrop)", "Fix height (eyedrop)" } },
                { "key.heightAdjustUp", new[] { "Subir altura fija (+0.1m)", "Increase fixed height (+0.1m)" } },
                { "key.heightAdjustDown", new[] { "Bajar altura fija (-0.1m)", "Decrease fixed height (-0.1m)" } },
                { "key.rampMode", new[] { "Modo de rampa", "Ramp mode" } },
                { "key.radial", new[] { "Menú radial (mantener)", "Radial menu (hold)" } },
                { "key.radialTools", new[] { "Cambiar página del wheel", "Switch radial page" } },
                { "key.radialConfirm", new[] { "Confirmar selección del wheel", "Confirm radial selection" } },
                { "key.radialCancel", new[] { "Cancelar wheel", "Cancel radial menu" } },
                { "radial.page.ops", new[] { "OPERACIONES", "OPERATIONS" } },
                { "radial.page.tools", new[] { "HERRAMIENTAS", "TOOLS" } },
                { "radial.tool.copy", new[] { "Copiar terreno", "Copy terrain" } },
                { "radial.tool.paste", new[] { "Pegar terreno", "Paste terrain" } },
                { "radial.tool.undo", new[] { "Deshacer", "Undo" } },
                { "radial.tool.redo", new[] { "Rehacer", "Redo" } },
                { "radial.tool.prev", new[] { "Stamp anterior", "Previous stamp" } },
                { "radial.tool.next", new[] { "Stamp siguiente", "Next stamp" } },
                { "radial.hint", new[] { "Soltar {0}: aplicar · {1}: herramientas · {2}: confirmar · {3}: cancelar", "Release {0}: apply · {1}: tools · {2}: confirm · {3}: cancel" } },
                { "radial.cancel", new[] { "Cancelar", "Cancel" } },
                { "key.give", new[] { "Obtener herramienta", "Get tool" } },
                { "key.menu", new[] { "Abrir este menu", "Open this menu" } },
                // History section
                { "hist.title", new[] { "Historial de deshacer (pasos guardados)", "Undo history (stored steps)" } },
                { "hist.exact", new[] { "Exacto (0-500):", "Exact (0-500):" } },
                { "hist.saved", new[] { "Guardados: deshacer {0}  ·  rehacer {1}", "Stored: undo {0}  ·  redo {1}" } },
                // Settings tab
                { "set.brush", new[] { "Brocha (click para cambiar)", "Brush (click to change)" } },
                { "set.sizes", new[] { "Tamaños (arrastrar)", "Sizes (drag)" } },
                // Settings section headers
                { "set.tool", new[] { "Herramienta", "Tool" } },
                { "set.view", new[] { "Vista", "View" } },
                { "set.vanilla", new[] { "Vanilla", "Vanilla" } },
                { "set.server", new[] { "Servidor (manda a los clientes)", "Server (wins on clients)" } },
                { "set.general", new[] { "General", "General" } },
                // Tool tint
                { "set.tooltint", new[] { "Teñir la herramienta", "Tint the tool" } },
                { "set.toolcolor", new[] { "Color de la herramienta", "Tool color" } },
                { "set.operation", new[] { "Operación", "Operation" } },
                { "set.shape", new[] { "Forma", "Shape" } },
                { "set.paint", new[] { "Pintura", "Paint" } },
                { "set.rampmode", new[] { "Modo rampa", "Ramp mode" } },
                { "set.smoothmode", new[] { "Modo smooth", "Smooth mode" } },
                { "set.language", new[] { "Idioma", "Language" } },
                { "set.radius", new[] { "Radio", "Radius" } },
                { "set.softness", new[] { "Suavidad", "Softness" } },
                { "set.angle", new[] { "Ángulo rampa", "Ramp angle" } },
                { "set.raisedelta", new[] { "Subir/bajar (m)", "Raise/lower (m)" } },
                { "set.paintstrength", new[] { "Fuerza pintura", "Paint strength" } },
                { "set.falloff", new[] { "Caída Raise/Lower", "Raise/Lower falloff" } },
                { "set.falloffpower", new[] { "Intensidad de caída", "Falloff strength" } },
                { "set.shoreoffset", new[] { "Costa sobre agua (m)", "Shore above water (m)" } },
                { "set.ramp.tools", new[] { "Herramientas de rampa", "Ramp tools" } },
                { "set.ramp.width", new[] { "Ancho de rampa", "Ramp width" } },
                { "set.ramp.hint", new[] { "Dos puntos: marca A y B. Ángulo fijo: B define dirección y longitud. Conectada: marca 3 o más puntos para trazar una rampa continua; pulsa Aplicar dos veces para crearla. Radio = ancho.", "Two-Point: mark A and B. Fixed Angle: B sets direction and length. Connected: mark 3 or more points for one continuous ramp; press Apply twice to build it. Radius = width." } },
                { "set.copy.hint", new[] { "En Copiar/Pegar, marca el área con Aplicar. El HUD te indica el siguiente paso; usa la pestaña Copiar/Pegar para previsualizar y colocar el terreno.", "In Copy/Paste, mark the area with Apply. The HUD shows the next step; use the Copy/Paste tab to preview and place terrain." } },
                { "set.fixedheight", new[] { "Altura fija (m):", "Fixed height (m):" } },
                { "set.fix", new[] { "Fijar", "Fix" } },
                { "set.release", new[] { "Liberar", "Release" } },
                { "set.snapshots", new[] { "Snapshots de terreno (backup)", "Terrain snapshots (backup)" } },
                { "set.snapshotempty", new[] { "(sin snapshots guardados)", "(no saved snapshots)" } },
                { "snap.saved", new[] { "Snapshot guardado: {0} ({1} zonas).", "Snapshot saved: {0} ({1} tiles)." } },
                { "snap.empty", new[] { "Nada que guardar: no hay ediciones en el historial.", "Nothing to save: no edits in history." } },
                { "snap.loaded", new[] { "Snapshot cargado: {0}.", "Snapshot loaded: {0}." } },
                { "snap.missing", new[] { "Sin terreno cargado en {0} zona(s): importa cerca del lugar.", "Loaded terrain missing in {0} tile(s): import near the place." } },
                { "snap.badfile", new[] { "Archivo inválido o versión incompatible.", "Invalid file or incompatible version." } },
                { "snap.ioerror", new[] { "Error de archivo: {0}", "File error: {0}" } },
                { "snap.max", new[] { "Máximo de snapshots alcanzado, borra alguno.", "Snapshot limit reached, delete one." } },
                { "snap.badname", new[] { "Nombre inválido.", "Invalid name." } },
                { "set.preview", new[] { "Vista previa", "Preview" } },
                { "set.guides", new[] { "Guías", "Guides" } },
                { "set.slopeheatmap", new[] { "Mapa de pendiente", "Slope heatmap" } },
                { "set.heightheatmap", new[] { "Mapa de altura", "Height heatmap" } },
                { "set.effects", new[] { "Efectos (polvo+sonido)", "Effects (dust+sound)" } },
                { "set.autogive", new[] { "Dar herramienta al entrar", "Give tool on enter" } },
                { "set.nocost", new[] { "Sin costo de construcción", "No build cost" } },
                { "set.protected", new[] { "Ignorar zonas protegidas", "Ignore protected zones" } },
                { "set.deltalimit", new[] { "Pasar el límite +-8 m", "Bypass the +-8 m limit" } },
                { "set.allow", new[] { "Server: permitir edición", "Server: allow editing" } },
                { "set.adminonly", new[] { "Server: solo admins", "Server: admins only" } },
                { "set.profiles", new[] { "Perfiles (guardar/cargar brocha)", "Profiles (save/load brush)" } },
                { "set.profilesave", new[] { "Guardar perfil", "Save profile" } },
                { "set.keybinds", new[] { "Atajos", "Keybinds" } },
                { "set.resetkeys", new[] { "Restablecer atajos", "Reset keybinds" } },
                { "set.stamps", new[] { "Stamps (copiar/pegar y guardar terreno)", "Stamps (copy/paste and save terrain)" } },
                { "stamp.workflow", new[] { "Apunta a la zona y pulsa Copiar terreno. Para pegar, inicia la vista previa, mueve la mira y vuelve a pulsar Pegar terreno (o Ctrl+Shift+V) para confirmar; Esc cancela. En Línea, Rectángulo y Polígono, marca primero el área con Aplicar.", "Aim at an area and choose Copy terrain. To paste, start the preview, aim at the destination, then choose Paste terrain again (or press Ctrl+Shift+V) to confirm; Esc cancels. For Line, Rectangle, and Polygon shapes, mark the area with Apply first." } },
                { "stamp.clipboard", new[] { "Portapapeles: {0}/{1}", "Clipboard: {0}/{1}" } },
                { "set.profilename", new[] { "Nombre:", "Name:" } },
                { "set.stampname", new[] { "Nombre stamp:", "Stamp name:" } },
                { "set.stampsave", new[] { "Guardar stamp", "Save stamp" } },
                { "set.stampload", new[] { "Cargar stamp:", "Load stamp:" } },
                { "set.stamploadbtn", new[] { "Cargar al portapapeles", "Load to clipboard" } },
                { "set.stampnoclip", new[] { "No hay stamp en el portapapeles. Copiá con Ctrl+Shift+C.", "No stamp on the clipboard. Copy with Ctrl+Shift+C." } },
                { "set.stampempty", new[] { "(sin stamps guardados)", "(no saved stamps)" } },
                { "set.stamptool", new[] { "Herramienta stamp", "Stamp tool" } },
                { "stamptool.main", new[] { "Igual a brocha", "Same as brush" } },
                { "stamptool.any", new[] { "Cualquiera", "Any" } },
                { "stamptool.pickaxe", new[] { "Pico", "Pickaxe" } },
                { "stamptool.rod", new[] { "Varita TerrainForge", "TerrainForge Rod" } },
                { "set.profileload", new[] { "Cargar", "Load" } },
                { "set.profileempty", new[] { "(sin perfiles guardados)", "(no saved profiles)" } },
                // Shape / mode / op short names
                { "shape.circle", new[] { "Círculo", "Circle" } },
                { "shape.square", new[] { "Cuadrado", "Square" } },
                { "shape.line", new[] { "Línea", "Line" } },
                { "shape.rect", new[] { "Rectángulo", "Rectangle" } },
                { "shape.poly", new[] { "Polígono", "Polygon" } },
                { "smooth.full", new[] { "Completo", "Full" } },
                { "smooth.bumps", new[] { "Solo lomos", "Bumps only" } },
                { "smooth.dips", new[] { "Solo hoyos", "Dips only" } },
                { "rampmode.two", new[] { "Dos puntos", "Two-point" } },
                { "rampmode.fixed", new[] { "Ángulo fijo", "Fixed angle" } },
                { "rampmode.path", new[] { "Conectada", "Connected path" } },
                { "flash.rampmode.path", new[] { "Modo de rampa: conectada", "Ramp mode: connected path" } },
                { "falloff.pointy", new[] { "Punta", "Pointy" } },
                { "falloff.flat", new[] { "Plano", "Flat" } },
                { "falloff.smooth", new[] { "Suave", "Smooth" } },
                { "falloff.linear", new[] { "Lineal", "Linear" } },
                { "shorefalloff.linear", new[] { "Lineal", "Linear" } },
                { "shorefalloff.smooth", new[] { "Suave", "Smooth" } },
                { "shorefalloff.flat", new[] { "Plano", "Flat" } },
                { "shorefalloff.pointy", new[] { "Punta", "Pointy" } },
                { "hud.flash.falloff", new[] { "Caída: {0}", "Falloff: {0}" } },
                { "key.op.flatten", new[] { "Operación: Aplanar", "Op: Flatten" } },
                { "key.op.raise", new[] { "Operación: Subir", "Op: Raise" } },
                { "key.op.lower", new[] { "Operación: Bajar", "Op: Lower" } },
                { "key.op.smooth", new[] { "Operación: Suavizar", "Op: Smooth" } },
                { "key.op.paint", new[] { "Operación: Pintar", "Op: Paint" } },
                { "key.op.restore", new[] { "Operación: Restaurar", "Op: Restore" } },
                { "key.op.ramp", new[] { "Operación: Rampa", "Op: Ramp" } },
                { "key.op.shore", new[] { "Operación: Costa", "Op: Shore" } },
                { "key.op.snow", new[] { "Pintura: nieve", "Paint: snow" } },
                { "key.cycleProfile", new[] { "Siguiente perfil", "Next profile" } },
                { "key.copyStamp", new[] { "Copiar zona de terreno (stamp)", "Copy terrain area (stamp)" } },
                { "key.pasteStamp", new[] { "Pegar zona de terreno (stamp, 2 pulsos)", "Paste terrain area (stamp, 2 clicks)" } },
                { "key.cycleStamp", new[] { "Siguiente stamp (historial)", "Next stamp (history)" } },
                { "key.cycleStampBack", new[] { "Stamp anterior (historial)", "Previous stamp (history)" } },
                { "key.slopeHeatmap", new[] { "Alternar mapa de pendiente", "Toggle slope heatmap" } },
                { "key.heightHeatmap", new[] { "Alternar mapa de altura", "Toggle height heatmap" } },
                { "flash.stamp.copied", new[] { "Stamp copiado: {0} tiles", "Stamp copied: {0} tiles" } },
                { "flash.stamp.empty", new[] { "Stamp vacío: nada para copiar", "Stamp empty: nothing to copy" } },
                { "flash.stamp.preview", new[] { "Pegar stamp: click para confirmar, rueda=rotar, Shift+rueda=escalar, Esc=cancelar", "Paste stamp: click to confirm, wheel=rotate, Shift+wheel=scale, Esc=cancel" } },
                { "flash.stamp.pasted", new[] { "Stamp pegado", "Stamp pasted" } },
                { "flash.stamp.cycled", new[] { "Stamp historial: {0}", "Stamp history: {0}" } },
                { "flash.stamp.noTool", new[] { "Herramienta stamp no equipada", "Stamp tool not equipped" } },
                { "hud.stampReady", new[] { "STAMP LISTO", "STAMP READY" } },
                { "hud.stampEmpty", new[] { "STAMP vacío — Ctrl+Shift+C copia la zona", "STAMP empty — Ctrl+Shift+C copies the area" } },
                { "hud.stampNoTool", new[] { "herramienta stamp no equipada", "stamp tool not equipped" } },
                { "hud.stampPasteHint", new[] { "Ctrl+Shift+V pega (2 pulsos)", "Ctrl+Shift+V to paste (2 clicks)" } },
                { "hud.path.idle", new[] { "SENDA: marca puntos", "PATH: mark points" } },
                { "hud.path.mark", new[] { "SENDA: {0} puntos (doble pulso cierra)", "PATH: {0} points (double-press closes)" } },
                { "hud.path", new[] { "senda", "path" } },
                { "msg.path.point", new[] { "Senda: punto {0}. Doble pulso para cerrar y aplicar.", "Path: point {0}. Double-press to close and apply." } },
                { "msg.path.done", new[] { "Senda aplicada ({0} puntos).", "Path applied ({0} points)." } },
                { "msg.path.fail", new[] { "Senda: sin terreno en la zona.", "Path: no terrain in the area." } },
                { "op.flatten", new[] { "Aplanar", "Flatten" } },
                { "op.raise", new[] { "Subir", "Raise" } },
                { "op.lower", new[] { "Bajar", "Lower" } },
                { "op.smooth", new[] { "Suavizar", "Smooth" } },
                { "op.paint", new[] { "Pintar", "Paint" } },
                { "op.copypaste", new[] { "Copiar / Pegar", "Copy / Paste" } },
                { "op.restore", new[] { "Restaurar", "Restore" } },
                { "op.ramp", new[] { "Rampa", "Ramp" } },
                { "op.shore", new[] { "Costa", "Shore" } },
                { "op.snow", new[] { "Nieve", "Snow" } },
                { "hud.snow.put", new[] { "poner nieve", "place snow" } },
                { "hud.snow.clear", new[] { "quitar nieve", "remove snow" } },
                // Help topics + bodies
                { "help.what.t", new[] { "Que hace", "What it does" } },
                { "help.what.b", new[] {
                    "TerrainForge esculpe el terreno libremente: sube/baja mas de 8 m, suaviza, crea rampas, pinta y restaura, sin las ataduras del pico o la azada vanilla. Los cambios se guardan en el mundo y se replican a los demas jugadores.",
                    "TerrainForge sculpts terrain freely: raise/lower beyond 8 m, smooth, build ramps, paint and restore, without pickaxe or vanilla hoe limits. Changes are saved in the world and replicated to other players." } },
                { "help.tool.t", new[] { "Herramienta", "Tool" } },
                { "help.tool.b", new[] {
                    "• 'TerrainForge Rod': la varita del mod (clon del martillo). Se te entrega automaticamente al entrar al mundo y se equipa sola; tambien podes obtenerla con la tecla \"Obtener herramienta\" o desde esta ventana.\n• Debe estar EQUIPADA para que la brocha funcione. Si el registro del objeto fallara, la brocha queda usable igual.",
                    "• 'TerrainForge Rod': the mod's wand (a Hammer clone). It is handed out automatically when you enter the world and equips itself; you can also get it with the \"Get tool\" key or from this window.\n• It must be EQUIPPED for the brush to work. If item registration ever failed, the brush stays usable anyway." } },
                { "help.ops.t", new[] { "Operaciones  (cambiar con \"Cambiar operacion\" o el menú radial)", "Operations  (change with \"Cycle operation\" or the radial menu)" } },
                { "help.ops.b", new[] {
                    "• Flatten: aplana a la altura apuntada (mantene Shift para nivelar a tus pies).\n• Raise: sube el terreno.\n• Lower: baja el terreno.\n• Smooth: promedia cada punto con sus vecinos y suaviza el terreno de verdad (bordes, escalones y ruido). Mantenelo para suavizar mas (cruza los bordes entre zonas sin abrir la malla). En AJUSTES elegis Completo, Solo lomos o Solo hoyos. Con forma Linea suavizas a lo largo de un camino.\n• Shore: aplana hacia el nivel del agua + un margen (playa). Ideal para costas; el margen se ajusta en AJUSTES.\n• Ramp: marca un punto A y luego un punto B con la tecla Aplicar; se crea una rampa recta entre ambos que une las dos alturas. El ancho es el radio de la brocha y \"Softness\" suaviza solo los bordes laterales. Con \"Modo de rampa\" en angulo fijo, B solo da direccion y largo: la altura sigue el angulo (sube o baja segun donde apuntes). En modo senda marcas una curva A→B→C (doble pulso con 3+ puntos cierra) y la rampa la sigue suavizada.\n• Paint: pinta el terreno (Dirt, Cultivated, Paved, hierba/vegetacion...).\n• Restore: devuelve el terreno a su estado original.\n• Paint incluye DeepSnow (con Shift la quita y restaura el original).\n• Copiar/Pegar es un modo propio en la rueda Q para seleccionar un área y colocar sellos de terreno.",
                    "• Flatten: levels to the aimed height (hold Shift to level to your feet).\n• Raise: raises terrain.\n• Lower: lowers terrain.\n• Smooth: averages each point with its neighbours and truly smooths terrain (edges, steps, noise). Hold to smooth more (crosses zone borders without opening seams). In SETTINGS choose Full, Bumps only or Dips only. With the Line shape you smooth along a path.\n• Shore: flattens toward the water level + a margin (beach). Ideal for coastlines; the margin is set in SETTINGS.\n• Ramp: mark point A then point B with the Apply key; a straight ramp joining both heights is built. Width is the brush radius and \"Softness\" softens only the side edges. With ramp \"Mode\" on fixed angle, B only sets direction and length: height follows the angle (up or down depending on your aim). In path mode you mark a curve A→B→C (double-press with 3+ points closes) and the ramp follows it smoothed.\n• Paint: paints terrain (Dirt, Cultivated, Paved, grass/vegetation...).\n• Restore: returns terrain to its original state.\n• Paint includes DeepSnow (hold Shift to remove it and restore the original).\n• Copy/Paste is a dedicated Q-wheel mode for selecting an area and placing terrain stamps." } },
                { "help.controls.t", new[] { "Controles", "Controls" } },
                { "help.controls.b", new[] {
                    "• Mantener \"Aplicar\" para esculpir (en Ramp: cada pulsacion marca un punto A o B).\n• \"Subir/Bajar radio\" o Ctrl + rueda del raton: tamano de la brocha (en Ramp y Linea es el ancho).\n• \"Cambiar forma\": circulo, cuadrado, linea, rectangulo o poligono. Linea/Rectangulo se marcan con dos pulsos de Aplicar (A y B); el poligono suma un punto por pulso y se cierra con doble pulso (3+ puntos). Los bordes se suavizan segun Softness. Con estas formas no se muestra el anillo circular, solo el contorno que marques.\n• \"Cambiar pintura\": tipo de pintura.\n• \"Nivelar a tus pies\" (Shift): Flatten toma esa altura en vez de la apuntada.\n• \"Fijar altura\": copia la altura apuntada y Flatten la usa siempre hasta liberarla (gana a Shift). Tambien podes escribirla en AJUSTES.\n• \"Modo de rampa\": dos puntos, angulo fijo o senda. Con angulo fijo, Alt + teclas de radio (o rueda) ajusta los grados.\n• \"Modo ajuste\": la rueda cambia el radio sin mover la camara (congela zoom, hotbar y mapa mientras esta activo).\n• Con tope del servidor, el radio muestra \"(tope)\".\n• Shift + rueda (o teclas de radio) = ajuste fino del radio (0.1 m).\n• Las guias se ven grises cuando el servidor o un ward bloquean la edicion.\n• \"Menú radial\" (mantener): elige la operacion con el raton y solta para confirmar (Esc cancela).\n• El HUD (abajo a la derecha) muestra operacion, radio, forma y pintura actuales, mas una segunda linea con altura apuntada, objetivo y pendiente en grados.\n• \"Deshacer\" (Ctrl+Z) revierte la ultima aplicacion y \"Rehacer\" (Ctrl+Y) la recupera. La profundidad del historial se ajusta en la pestaña TECLAS (0 lo desactiva y borra lo guardado).\n• Al marcar líneas, rectángulos, polígonos y rampas verás medidas en vivo en el HUD (distancia, desnivel, ángulo, área).",
                    "• Hold \"Apply\" to sculpt (in Ramp: each press marks point A or B).\n• \"Radius up/down\" or Ctrl + mouse wheel: brush size (in Ramp and Line it is the width).\n• \"Cycle shape\": circle, square, line, rectangle or polygon. Line/Rectangle are marked with two Apply presses (A and B); the polygon adds one point per press and closes on double-press (3+ points). Edges soften with Softness. With these shapes no round ring is shown, only the outline you mark.\n• \"Cycle paint\": paint type.\n• \"Level to feet\" (Shift): Flatten uses that height instead of the aimed one.\n• \"Fix height\": copies the aimed height and Flatten always uses it until released (wins over Shift). You can also type it in SETTINGS.\n• \"Ramp mode\": two-point, fixed angle or path. With fixed angle, Alt + radius keys (or wheel) adjust the degrees.\n• \"Adjust mode\": the wheel changes radius without moving the camera (freezes zoom, hotbar and map while active).\n• With a server cap, the radius shows \"(capped)\".\n• Shift + wheel (or radius keys) = fine radius adjust (0.1 m).\n• Guides look grey when the server or a ward blocks editing.\n• \"Radial menu\" (hold): pick the operation with the mouse and release to confirm (Esc cancels).\n• The HUD (bottom right) shows current operation, radius, shape and paint, plus a second line with aimed height, target and slope in degrees.\n• \"Undo\" (Ctrl+Z) reverts the last application and \"Redo\" (Ctrl+Y) recovers it. History depth is set in the KEYS tab (0 disables it and clears the stored steps).\n• While marking lines, rectangles, polygons and ramps you will see live measurements in the HUD (distance, rise, angle, area)." } },
                { "help.preview.t", new[] { "Vista previa", "Preview" } },
                { "help.preview.b", new[] {
                    "• La linea solida marca el borde real: donde se genera terreno.\n• Guias tenues alrededor (mismo color de cada operacion): zona de fuerza plena, radio maximo y distancia maxima de apuntado.\n• En Ramp, marca A y B y veras el corredor real (las dos lineas del ancho).\n• El borde se tiñe de rojo donde el terreno empina (guia de pendiente).\n• En Flatten/Shore veras un anillo al plano objetivo (marca de nivel).\n• Se pueden desactivar en la config (Preview > ShowGuides).",
                    "• The solid line marks the real edge: where terrain is generated.\n• Faint guides around it (each operation's color): full-strength zone, maximum radius and maximum aim distance.\n• In Ramp, mark A and B to see the real corridor (both width lines).\n• The edge tints red where terrain gets steep (slope guide).\n• In Flatten/Shore you will see a ring at the target plane (level marker).\n• They can be disabled in the config (Preview > ShowGuides)." } },
                { "help.stamp.t", new[] { "Copiar / pegar terreno (stamps)", "Copy / paste terrain (stamps)" } },
                { "help.stamp.b", new[] {
                    "• Copiar: elige Copiar/Pegar en la rueda Q. Rectángulo marca A y B con Aplicar; Línea marca sus extremos; Polígono suma vértices y copia con doble pulso tras 3+ puntos; Círculo/Cuadrado copian la huella de la brocha. Ctrl+Shift+C sigue el mismo flujo.\n• Pegar: pulsa Pegar terreno o Ctrl+Shift+V para iniciar la vista previa. Mueve la mira y vuelve a usar Pegar terreno/Ctrl+Shift+V para confirmar. Rueda = rotar, Shift+rueda = escalar, Esc cancela.\n• El portapapeles conserva hasta 10 stamps; los botones Anterior/Siguiente recorren el historial.\n• En esta misma pestaña puedes guardar stamps con nombre, cargar los guardados al portapapeles o borrarlos.\n• \"Herramienta stamp\" en OPCIONES elige qué herramienta permite copiar/pegar: TerrainForge, pico o cualquiera.",
                    "• Copy: choose Copy/Paste from the Q wheel. Rectangle marks A and B with Apply; Line marks its endpoints; Polygon adds vertices and copies on a double-press after 3+ points; Circle/Square copy the brush footprint. Ctrl+Shift+C follows the same flow.\n• Paste: choose Paste terrain or press Ctrl+Shift+V to start the preview. Aim at the destination and choose Paste terrain/press Ctrl+Shift+V again to confirm. Wheel = rotate, Shift+wheel = scale, Esc cancels.\n• The clipboard keeps up to 10 stamps; Previous/Next cycle through them.\n• In the same tab, you can save named stamps, load saved ones to the clipboard, or delete them.\n• \"Stamp tool\" in OPTIONS selects which tool allows copy/paste: TerrainForge, pickaxe, or any." } },
                { "help.mp.t", new[] { "Multijugador", "Multiplayer" } },
                { "help.mp.b", new[] {
                    "• El mod debe estar en el cliente Y en el servidor.\n• \"UnlockDeltaLimit\" debe estar activado tambien en el servidor; si no, el servidor recorta los cambios de mas de 8 m.\n• Radios muy grandes pueden desincronizar a clientes que no tengan el mod.\n• Autoridad del servidor: si el servidor tiene el mod, su seccion \"Server\" manda sobre los clientes (AllowTerrainEdits, AdminOnlyEdits). En servidores SIN el mod, tu config local sigue valiendo y la herramienta funciona para probar.\n• Con RespectWards activado no se edita dentro de wards ajenos.\n• RadiusCap/DeltaCap dejan al servidor imponer radio y desnivel maximos (se ven recortados en HUD y preview).",
                    "• The mod must be on the client AND the server.\n• \"UnlockDeltaLimit\" must also be enabled on the server; otherwise the server clamps changes beyond 8 m.\n• Very large radii may desync clients without the mod.\n• Server authority: if the server runs the mod, its \"Server\" section rules over clients (AllowTerrainEdits, AdminOnlyEdits). On servers WITHOUT the mod, your local config still applies and the tool keeps working for testing.\n• With RespectWards on, editing inside other players' wards is blocked.\n• RadiusCap/DeltaCap let the server enforce maximum radius and delta (seen clamped in HUD and preview)." } },
                { "help.save.t", new[] { "Recargar / guardar", "Reload / save" } },
                { "help.save.b", new[] {
                    "Las teclas se guardan solas al reasignarlas. La pestaña AJUSTES edita todo en vivo (operacion, forma, pintura, modos, tamaños e interruptores) y se guarda al cerrar el menu. Todo vive tambien en el archivo de configuracion del mod (BepInEx/config). Los snapshots de terreno (copias de seguridad) se guardan y cargan en AJUSTES. Sin nombre se usa la fecha y hora.",
                    "Keys save themselves when rebound. The SETTINGS tab edits everything live (operation, shape, paint, modes, sizes and switches) and saves when the menu closes. Everything also lives in the mod config file (BepInEx/config). Terrain snapshots (backups) are saved and loaded in SETTINGS. Empty names get date and time." } },
                // Brush messages
                { "msg.ramp.markA", new[] { "Rampa: punto A marcado. Apunta al punto B y volve a pulsar aplicar.", "Ramp: point A marked. Aim at point B and press apply again." } },
                { "msg.ramp.close", new[] { "Rampa: los puntos A y B estan demasiado cerca.", "Ramp: points A and B are too close." } },
                { "msg.ramp.doneAB", new[] { "Rampa creada de A a B.", "Ramp created from A to B." } },
                { "msg.ramp.fail", new[] { "Rampa: no se pudo crear (puntos muy cortos o sin terreno).", "Ramp: could not be created (points too short or no terrain)." } },
                { "msg.ramp.fixmark", new[] { "Rampa ángulo {0}°: punto A marcado. Apunta la dirección y volve a pulsar aplicar.", "Ramp {0}° angle: point A marked. Aim the direction and press apply again." } },
                { "msg.ramp.fixdone", new[] { "Rampa {0}° creada ({1} {2:0.0} m en {3:0.0} m).", "{0}° ramp created ({1} {2:0.0} m over {3:0.0} m)." } },
                { "msg.ramp.up", new[] { "sube", "rises" } },
                { "msg.ramp.down", new[] { "baja", "drops" } },
                { "msg.line.markA", new[] { "Línea: punto A marcado. Apunta a B y volve a pulsar aplicar.", "Line: point A marked. Aim at B and press apply again." } },
                { "msg.line.close", new[] { "Línea: A y B estan demasiado cerca.", "Line: A and B are too close." } },
                { "msg.line.done", new[] { "Línea aplicada.", "Line applied." } },
                { "msg.line.fail", new[] { "Línea: sin terreno en la zona.", "Line: no terrain in the area." } },
                { "msg.rect.markA", new[] { "Rectángulo: esquina A marcada. Apunta a la esquina B y volve a pulsar.", "Rectangle: corner A marked. Aim at corner B and press again." } },
                { "msg.rect.small", new[] { "Rectángulo: demasiado chico (mínimo 1×1 m).", "Rectangle: too small (1×1 m minimum)." } },
                { "msg.rect.done", new[] { "Rectángulo aplicado.", "Rectangle applied." } },
                { "msg.rect.fail", new[] { "Rectángulo: sin terreno en la zona.", "Rectangle: no terrain in the area." } },
                { "msg.poly.point", new[] { "Polígono: punto {0}. Doble pulso para cerrar y aplicar.", "Polygon: point {0}. Double-press to close and apply." } },
                { "msg.poly.done", new[] { "Polígono aplicado ({0} puntos).", "Polygon applied ({0} points)." } },
                { "msg.poly.fail", new[] { "Polígono: sin terreno en la zona.", "Polygon: no terrain in the area." } },
                // Flash + HUD
                { "flash.undo", new[] { "Deshecho (quedan {0})", "Undone (left {0})" } },
                { "flash.undo.op", new[] { "Deshecho: {0} (quedan {1})", "Undone: {0} ({1} left)" } },
                { "flash.redo", new[] { "Rehecho (quedan {0})", "Redone (left {0})" } },
                { "flash.redo.op", new[] { "Rehecho: {0} (quedan {1})", "Redone: {0} ({1} left)" } },
                { "flash.nothing.undo", new[] { "Nada que deshacer", "Nothing to undo" } },
                { "flash.nothing.redo", new[] { "Nada que rehacer", "Nothing to redo" } },
                { "flash.picked", new[] { "Altura fijada: {0} m (Flatten la usará)", "Height fixed: {0} m (Flatten will use it)" } },
                { "flash.released", new[] { "Altura liberada (Flatten vuelve a la apuntada)", "Height released (Flatten back to aimed)" } },
                { "flash.rampmode.fixed", new[] { "Modo de rampa: ángulo fijo {0}°", "Ramp mode: fixed angle {0}°" } },
                { "flash.rampmode.two", new[] { "Modo de rampa: dos puntos", "Ramp mode: two-point" } },
                { "flash.angle", new[] { "Ángulo de rampa: {0}°", "Ramp angle: {0}°" } },
                { "srv.adminonly", new[] { "Solo administradores pueden editar terreno en este servidor.", "Only admins may edit terrain on this server." } },
                { "srv.blocked", new[] { "El servidor no permite editar terreno.", "This server does not allow terrain editing." } },
                { "srv.ward", new[] { "Sin acceso en esta zona protegida (ward).", "No access in this protected zone (ward)." } },
                { "set.respectwards", new[] { "RespectWards (wards ajenos)", "RespectWards (foreign wards)" } },
                { "set.radiuscap", new[] { "Tope radio (server)", "Radius cap (server)" } },
                { "set.deltacap", new[] { "Tope desnivel (server)", "Delta cap (server)" } },
                { "set.chunkcomps", new[] { "Tiles por frame", "Tiles per frame" } },
                { "flash.applying", new[] { "Aplicando…", "Applying…" } },
                { "hud.capped", new[] { "tope", "capped" } },
                { "key.adjust", new[] { "Modo ajuste", "Adjust mode" } },
                { "hud.adjust", new[] { "MODO AJUSTE", "ADJUST MODE" } },
                { "flash.adjust.on", new[] { "Modo ajuste: la rueda cambia el radio (zoom congelado)", "Adjust mode: wheel changes radius (zoom frozen)" } },
                { "flash.adjust.off", new[] { "Modo ajuste: apagado", "Adjust mode: off" } },
                { "key.freecam", new[] { "Cámara libre", "Free camera" } },
                { "hud.freecam", new[] { "CÁMARA LIBRE · WASD vuela · rueda: velocidad · F salir", "FREE CAMERA · WASD fly · wheel: speed · F exit" } },
                { "hud.freecamHint", new[] { "F: cámara libre para zonas grandes", "F: free camera for large areas" } },
                { "hud.freecam.banner", new[] {
                    "MODO CÁMARA LIBRE · WASD+ratón vuela (rueda: velocidad) · clic para moldear · Ctrl+rueda: radio · F para salir",
                    "FREE CAMERA MODE · WASD+mouse to fly (wheel: speed) · click to sculpt · Ctrl+wheel: radius · F to exit" } },
                { "hud.freecam.dist", new[] { "objetivo a {0} m", "target at {0} m" } },
                { "flash.freecam.off", new[] { "Cámara libre: OFF", "Free camera: OFF" } },
                { "flash.freecam.on", new[] { "Cámara libre: ON (F para salir, rueda = velocidad)", "Free camera: ON (F to exit, wheel = speed)" } },
                { "tut.1", new[] { "TerrainForge listo: mantén Aplicar para esculpir.", "TerrainForge ready: hold Apply to sculpt." } },
                { "tut.2", new[] { "Q = operaciones · F5 = menú y ayuda.", "Q = operations · F5 = menu and help." } },
                { "hud.ward", new[] { "WARD: SIN ACCESO", "WARD: NO ACCESS" } },
                { "hud.radius", new[] { "radio", "radius" } },
                { "hud.undo", new[] { "deshacer", "undo" } },
                { "hud.redo", new[] { "rehacer", "redo" } },
                { "hud.blocked", new[] { "SERVER: BLOQUEADO", "SERVER: BLOCKED" } },
                { "hud.rampA", new[] { "RAMPA: punto A fijado, apunta a B", "RAMP: point A set, aim at B" } },
                { "hud.aimed", new[] { "apuntado", "aimed" } },
                { "hud.target", new[] { "objetivo", "target" } },
                { "hud.fixed", new[] { "fija", "fixed" } },
                { "hud.slope", new[] { "pendiente", "slope" } },
                { "hud.avg", new[] { "promedio vecinos", "neighbor average" } },
                { "hud.bumps", new[] { "solo lomos", "bumps only" } },
                { "hud.dips", new[] { "solo hoyos", "dips only" } },
                { "hud.shore", new[] { "costa", "shore" } },
                { "hud.original", new[] { "original", "original" } },
                { "hud.perapp", new[] { "m/aplic.", "m/apply" } },
                { "hud.markAB", new[] { "marca A y B", "mark A and B" } },
                { "hud.markA", new[] { "marca A", "mark A" } },
                { "hud.markpts", new[] { "marca puntos", "mark points" } },
                { "hud.line.idle", new[] { "LÍNEA: marca A y B", "LINE: mark A and B" } },
                { "hud.line.mark", new[] { "LÍNEA: A fijado, apunta a B", "LINE: A set, aim at B" } },
                { "hud.rect.idle", new[] { "RECT: marca dos esquinas", "RECT: mark two corners" } },
                { "hud.rect.mark", new[] { "RECT: esquina A fijada, apunta a B", "RECT: corner A set, aim at B" } },
                { "hud.copy.circle", new[] { "APLICAR copia el área de la mira y el radio", "APPLY copies the aimed area and radius" } },
                { "hud.copy.rectA", new[] { "COPIA: marca la esquina A", "COPY: mark corner A" } },
                { "hud.copy.rectB", new[] { "COPIA: A fijada, apunta y marca B", "COPY: A set, aim and mark B" } },
                { "hud.copy.lineA", new[] { "COPIA: marca el inicio de la línea", "COPY: mark line start" } },
                { "hud.copy.lineB", new[] { "COPIA: inicio fijado, apunta al final", "COPY: start set, aim at endpoint" } },
                { "hud.copy.poly", new[] { "COPIA: {0} puntos; doble pulso cierra", "COPY: {0} points; double-press closes" } },
                { "hud.poly.idle", new[] { "POLI: marca puntos", "POLY: mark points" } },
                { "hud.poly.mark", new[] { "POLI: {0} puntos (doble pulso cierra)", "POLY: {0} points (double-press closes)" } },
                { "hud.ramp.fixed.idle", new[] { "rampa {0}°: marca A", "{0}° ramp: mark A" } },
                { "hud.ramp.fixed.mark", new[] { "{0}°: A → B", "{0}°: A → B" } },
                { "act.clearmarks", new[] { "Limpiar marcas", "Clear marks" } },
                { "act.copystamp", new[] { "Copiar terreno", "Copy terrain" } },
                { "act.diag", new[] { "Diagnóstico material (log)", "Material diagnostic (log)" } },
                { "act.diagfail", new[] { "Sin terreno bajo el cursor", "No terrain under cursor" } },
                { "act.eyedrop", new[] { "Fijar altura", "Fix height" } },
                { "act.pastestamp", new[] { "Pegar terreno", "Paste terrain" } },
                { "act.stampprev", new[] { "Stamp anterior (historial)", "Previous stamp (history)" } },
                { "hud.coords", new[] { "Pos", "Pos" } },
                { "keys.resetdone", new[] { "Atajos restablecidos a los valores por defecto.", "Keybinds reset to defaults." } },
                { "set.blockvanilla", new[] { "Bloquear tools terrain vanilla", "Block vanilla terrain tools" } },
                { "set.quick", new[] { "Acciones rápidas", "Quick actions" } },
            };

        private static Type s_localizationType;
        private static PropertyInfo s_instanceProp;
        private static MethodInfo s_getSelectedLanguage;
        private static MethodInfo s_localizeMethod;

        private static void EnsureLocalizationReflection()
        {
            if (s_localizationType != null)
                return;
            try
            {
                s_localizationType = Type.GetType("Localization, assembly_valheim");
                if (s_localizationType == null)
                {
                    // Try without assembly name
                    s_localizationType = Type.GetType("Localization");
                }
                if (s_localizationType != null)
                {
                    s_instanceProp = s_localizationType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
                    s_getSelectedLanguage = s_localizationType.GetMethod("GetSelectedLanguage", BindingFlags.Public | BindingFlags.Instance);
                    s_localizeMethod = s_localizationType.GetMethod("Localize", BindingFlags.Public | BindingFlags.Instance);
                }
            }
            catch { }
        }

        private static string GetGameLanguage()
        {
            EnsureLocalizationReflection();
            if (s_instanceProp == null || s_getSelectedLanguage == null)
                return "English";
            try
            {
                object instance = s_instanceProp.GetValue(null);
                if (instance != null)
                {
                    return s_getSelectedLanguage.Invoke(instance, null) as string ?? "English";
                }
            }
            catch { }
            return "English";
        }

        public static string Localize(string token)
        {
            EnsureLocalizationReflection();
            if (s_instanceProp == null || s_localizeMethod == null)
                return token;
            try
            {
                object instance = s_instanceProp.GetValue(null);
                if (instance != null)
                    return (s_localizeMethod.Invoke(instance, new object[] { token }) as string) ?? token;
            }
            catch { }
            return token;
        }

        public static string T(string key)
        {
            string[] a;
            if (!s_texts.TryGetValue(key, out a) || a == null)
                return key;

            ModLanguage lang = ResolveLanguage();

            // Spanish and English live in the base [es, en] table.
            if (lang == ModLanguage.Español)
                return a.Length > 0 && a[0] != null ? a[0] : key;

            // Other languages use the per-language override table; missing keys
            // fall back to English so a partial translation never shows a raw key.
            if (lang != ModLanguage.English && s_extra != null &&
                s_extra.TryGetValue(lang, out var table) && table != null &&
                table.TryGetValue(key, out string tr) && !string.IsNullOrEmpty(tr))
                return tr;

            return a.Length > 1 && a[1] != null ? a[1] : key;
        }

        /// <summary>
        /// Resolves the language to use: the config choice, or the game's own
        /// language when set to Auto.
        /// </summary>
        private static ModLanguage ResolveLanguage()
        {
            try
            {
                TerrainForgePlugin p = TerrainForgePlugin.Instance;
                if (p != null && p.Language != null)
                {
                    ModLanguage cfg = p.Language.Value;
                    if (cfg != ModLanguage.Auto)
                        return cfg;
                }
            }
            catch { }
            return MapGameLanguage(GetGameLanguage());
        }

        /// <summary>
        /// Maps the game's localization language name (Localization.GetSelectedLanguage)
        /// to a ModLanguage. Robust to naming differences across game versions.
        /// </summary>
        private static ModLanguage MapGameLanguage(string gameLang)
        {
            string l = (gameLang ?? string.Empty).ToLowerInvariant();
            if (l.Contains("traditional")) return ModLanguage.繁體中文;
            if (l.Contains("spanish") || l.Contains("españ")) return ModLanguage.Español;
            if (l.Contains("french") || l.Contains("franç")) return ModLanguage.Français;
            if (l.Contains("german") || l.Contains("deutsch")) return ModLanguage.Deutsch;
            if (l.Contains("italian") || l.Contains("italiano")) return ModLanguage.Italiano;
            if (l.Contains("portug")) return ModLanguage.Português;
            if (l.Contains("russian") || l.Contains("русск")) return ModLanguage.Русский;
            if (l.Contains("ukrain") || l.Contains("україн")) return ModLanguage.Українська;
            if (l.Contains("turk") || l.Contains("türk")) return ModLanguage.Türkçe;
            if (l.Contains("polish") || l.Contains("polski")) return ModLanguage.Polski;
            if (l.Contains("japan") || l.Contains("日本語")) return ModLanguage.日本語;
            if (l.Contains("korean") || l.Contains("한국")) return ModLanguage.한국어;
            if (l.Contains("chinese") || l.Contains("中文")) return ModLanguage.中文;
            return ModLanguage.English;
        }

        public static string F(string key, params object[] args)
        {
            try { return string.Format(T(key), args); }
            catch { return T(key); }
        }

        public static string OpName(TerrainOperation op)
        {
            switch (op)
            {
                case TerrainOperation.Flatten: return T("op.flatten");
                case TerrainOperation.Raise: return T("op.raise");
                case TerrainOperation.Lower: return T("op.lower");
                case TerrainOperation.Smooth: return T("op.smooth");
                case TerrainOperation.Paint: return T("op.paint");
                case TerrainOperation.CopyPaste: return T("op.copypaste");
                case TerrainOperation.Restore: return T("op.restore");
                case TerrainOperation.Ramp: return T("op.ramp");
                case TerrainOperation.Shore: return T("op.shore");
                case TerrainOperation.Snow: return T("op.snow");
                default: return op.ToString();
            }
        }
    }

    /// <summary>
    /// Fully describes a single brush application. Built by <see cref="TerrainBrush"/>
    /// from config + input state and consumed by <see cref="TerrainEngine"/>.
    /// </summary>
}
