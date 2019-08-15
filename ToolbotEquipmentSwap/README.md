# ToolbotEquipmentSwap
Lets you swap your equipment when playing as MUL-T with a dedicated key. It also optionally stops MUL-T's Retool ability from swapping his equipment slots. Configurable.

## Installation
1. Move `ToolbotEquipmentSwap.dll` to `Risk of Rain 2\BepInEx\plugins`.

*You may optionally make a folder and put the .dll file inside of it for organizational purposes.*
## Configuration
1. Install the plugin.
2. Run the game and a configuration file will be generated at `Risk of Rain 2\BepInEx\config`
3. Open `com.unordinal.toolbotequipmentswap.cfg` with any text editor.
4. Edit the settings to your preference.

### Settings
*Server-side settings apply to everyone in the server based on the host's config file setting.*

`SwapKey`: The key to swap between MUL-T's equipment slots. *(Default: `X`)*

`StopAutoSwap` (Server-side): Whether to stop the equipment slot changing when using MUL-T's Retool ability. *(Default: `true`)*
- If this is true, clients will not be able to swap equipment slots as MUL-T at all unless they also have the mod installed.
