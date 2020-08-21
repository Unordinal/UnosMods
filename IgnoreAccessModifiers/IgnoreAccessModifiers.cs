/*
 * You can publicize the ror2 assembly by following these steps:
 * - Go to: https://github.com/Reinms/Stubber-Publicizer
 * - Download latest release and unzip
 * - Locate consolestubber.exe in the appropriate folder for your operating system
 * - Open a new file explorer window
 * - Navigate to {ror2folder}/Risk of Rain 2_Data/Managed
 * - Locate Assembly-CSharp.dll
 * - Click and drag this file directly onto consolestubber.exe, a console window will appear
 * - When prompted to output a reference assembly, enter a positive response t | true | y | yes
 * - When prompted to publicize the reference assembly, enter a positive response
 * - When prompted to remove readonly from reference assembly, enter a positive response
 * - When prompted for an output folder, enter stubs (or any other unused subfolder name)
 * - When prompted for anything else, enter a negative response f | false | n | no
 * - When prompted to confirm, verify that the values are correct. Enter a positive response to continue, and a negative response to restart
 * - The tool will run
 * - If any errors occur, let me know or make a github issue
 * - Once complete, close the window and locate Assembly-CSharp.refstub.dll in the subfolder you specified
 * - Rename it to Assembly-CSharp.dll  and move it to your libs or references folder for your mods
 * - Inside your plugin, create a new empty source file
 * - Add the following code to it:
 *      using System.Security;
 *      using System.Security.Permissions;

 *      [module: UnverifiableCode]
 *      [assembly: SecurityPermission( SecurityAction.RequestMinimum, SkipVerification = true )]

 * - You can now access any private, internal, or readonly types, methods, properties, fields, and events in the RoR2 assembly
 */

// Add this file as a link to any project that uses a publicized .dll.
// To add as link: Right Click Project > Add > Existing Item > Publicizer/Publicizer.cs > Dropdown on Add Button > Add as Link

// SecurityPermision set to minimum and SkipVerification set to true
// for skipping access modifiers check from the mono JIT
// The same attributes are added to the assembly when ticking
// Unsafe Code in the Project settings
// This is done here to allow an explanation of the trick and
// not in an outside source you could potentially miss.

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete