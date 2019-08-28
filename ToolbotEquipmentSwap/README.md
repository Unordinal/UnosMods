# ToolbotEquipmentSwap
Lets you swap your equipment when playing as MUL-T with a dedicated key. It also optionally stops MUL-T's Retool ability from swapping his equipment slots. Configurable.

*Note: The host **must** have the mod for it to work for everyone. Clients without the mod are unaffected.*

## Installation
1. Move `ToolbotEquipmentSwap.dll` to `Risk of Rain 2\BepInEx\plugins`.

*You may optionally make a folder and put the .dll file inside of it for organizational purposes.*
## Configuration
1. Install the plugin.
2. Run the game and a configuration file will be generated at `Risk of Rain 2\BepInEx\config`
3. Open `com.unordinal.toolbotequipmentswap.cfg` with any text editor.
4. Edit the settings to your preference.

### Settings
`SwapKey`: The key to swap between MUL-T's equipment slots. *(Default: `X`)*

`StopAutoSwap`: Whether to stop the equipment slot changing when using MUL-T's Retool ability. *(Default: `true`)*

## Changelog
### `1.0.1 | 2019-8-27`
- Client's own configuration for the setting `StopAutoSwap` now apply to them instead of the host's setting taking precedence. Unmodded clients are now completely unaffected and can swap equipment using Retool no matter the host's configuration.
### `1.0.0 | 2019-8-11`
- Initial release.
