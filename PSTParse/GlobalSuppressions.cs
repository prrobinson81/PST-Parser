//------------------------------------------------------------------------
// <remarks>
// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.
// </remarks>
// <author>Peter Robinson (CNH2\nt084)</author>
//------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "I like regions.")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Have grouped Interfaces, Unicode, and ANSI implementations of NDB-layer components into single file for organizational simplicity.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1634:FileHeaderMustShowCopyright", Justification = "Project is branched from open-source, there is no copyright.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Enum is unused.", Scope = "member", Target = "~T:PSTParse.Message_Layer.PropType")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Have grouped Interfaces, Unicode, and ANSI implementations of NDB-layer components into single file for organizational simplicity.")]
[assembly: SuppressMessage("Compiler", "CS1591", Justification = "Enum is unused.", Scope = "type", Target = "~T:PSTParse.Message_Layer.PropType")]