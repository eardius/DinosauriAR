# DinosauriAR

Erstellt mit Unity 2018.2.4f1 


## Nutzerhinweise
Der fertige Build ist "Builds\ARCorePortalWithImageDetection.apk"
Dieser sollte auf jedem ARCore-fähigen Handy funktionieren (getestet wurde auf einem Google Pixel 2).

Für Debug-Zwecke kann die Anwendung auch aus dem Editor gestartet werden. 
Dafür muss das Smartphone per USB an den PC angeschlossen und USB-Debugging eingeschaltet werden.
Mit Hilfe des Instant Previews (wird automatisch installiert) wird der Play-Mode auf dem Smartphone ausgeführt. Dabei kann es allerdings zu Fehlern führen, die im Build nicht vorhanden sind (z.B. Interaktion mit GUI-Elementen kann nur per Maus im Editor stattfinden, nicht auf dem Bildschirm des Smartphones).

Das Starten der Anwendung im Editor ohne ein ARCore-fähiges Handy ist eine weitere Möglichkeit. 
Dabei muss folgendes beachtet werden:
- DebugCamera aktivieren
- StonePortal aus dem Ordner Prefabs in die Szene ziehen
  - Die DebugCamera als Device des PortalController (Skript) des PortalWindow (Kind vom StonePortal) referenzieren
- ARCoreDevice deaktivieren
