# [ Risk of Rain 2 ] mod template
a very minimal mod template for copy-paste setup

## checklist
- Rename the `.csproj` in `src/`
- Edit `src/Plugin.cs`
    - Define an appropriate `Name` *(and `Author`, if you are not me)*
    - Rename the namespace to chosen plugin name
- Edit `Thunderstore/manifest.json`
    - Replace `name` with chosen plugin name *(and `author`, if you are not me)*
    - Replace `website_url` with proper url *(or empty string)*
    - Write a proper `description`
- Add `Thunderstore/icon.png` *(256 x 256)*
- Populate *(or delete)* `CHANGELOG.md`
- Populate `README.md`
