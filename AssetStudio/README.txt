This is a slightly modified copy of AssetStudio v0.15.0 (commit 1766dcb)

https://github.com/Perfare/AssetStudio

The changes are:
- added m_IsActive property to GameObjects
- disabled reading Shader assets to speed up initialization
- added buffering to asset reading
- added UnpackUnityCrunchInit / UnpackUnityCrunchLevelData to extract
  only a specific mipmap
