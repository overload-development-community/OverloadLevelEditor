# Overload Level Editor

[Overload](https://playoverload.com) is a registered trademark of [Revival Productions, LLC](https://www.revivalprod.com).

Source code to Overload Level Editor (OLE) and Decal Mesh Editor (DMeshEditor)

#### How to run

- Download and extract the latest version (https://github.com/overload-development-community/OverloadLevelEditor/releases/download/v1.1.4.0/OverloadLevelEditor-1.1.4.0.zip) to a local folder.

- Run `OverloadLevelEditor.exe -datadir F:\SteamLibrary\steamapps\common\Overload\OverloadLevelEditor -gamedir F:\SteamLibrary\steamapps\common\Overload` (change path to datadir to your installed OverloadLevelEditor folder and gamedir to your installed Overload folder containing Overload.exe)

#### Improvements

- Displays game models for entities (by Arne)
- Application icon (by CHILLYBUS)
- Fix for NaN UV errors (by Arne)
- Fix lava particle effect size (by kevin)
- Align copy/paste by edge (by kevin)
- UV Editor window UI location/visible tweaks (by Sirius)
- Fix quaternion conversion used for entity rotation (by Arne)
- Add "Bisect Polygon" button (by Sirius)
- Locale independent hunter/falcon export (by Arne)
- Fix export with portal with lava slave side (by Arne)
- Obj Import Improvements (by Arne)
- Make texture name label in DMeshEditor work (by Arne)
- Fix invisible textures in DMeshEditor (by Arne)
- Allow cycling backwards on the Entity Pivot field (by Arne)
- Add color dialog to light color field (by Arne)
- Fix degenerate cases; make drag mark update marked vertex/face counts correctly (by Sirius)
- Fix display of textures in decal list (by Arne)
- Show current entity coordinates in Entity Coords input box (by Sirius)
- Fix inverted default segment UV orders (by kevin)
- Custom reflection probe options (by kevin)
  - Trigger entity subtype for placing reflection probes directly instead of through LevelPost
  - Checkbox in level custom info to include or exclude default chunk-based reflection probes on export
  - Exported reflection probes are always forced on

#### How to build

OverloadLevelEditor.sln should be buildable on Visual Studio 2017+
