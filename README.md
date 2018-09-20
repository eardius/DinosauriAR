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

## Anwendung
In der Anwendung muss die Kamera des Smartphones erst mindestens eine horizontale Fläche erkennen. Danach kann der Benutzer mittels eines Taps auf dem Bildschirm das Portal in die andere/virtuelle Welt erstellen und hindurch gehen. 

Gleichzeitig können zwei Bilder augmentiert werden:
https://github.com/eardius/DinosauriAR/raw/master/Assets/ARCore%20Images/plant.jpg
https://github.com/eardius/DinosauriAR/raw/master/Assets/ARCore%20Images/steak.png

Das Erkennen der Bilder kann sich manchmal als etwas schwierig erweisen. Am Besten funktioniert es, wenn die Kamera direkt über dem Bild gehalten und minimal geschwenkt wird. Wenn sich die augmentierten Nahrungsmittel und das Smartphone in der virtuellen Welt befinden, sollten die Dinosaurier, wenn sie in der Nähe sind, darauf reagieren. 
