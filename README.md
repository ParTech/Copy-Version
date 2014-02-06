Copy Version module
===================

Description
-----------
This module adds commands to the Content Editor that allow users to copy and paste the latest version of an item instead of the entire item.
After installation, the commands are accessible from the context menu in the content tree.
The following commands will be available after installation:

Copy version / Paste version
...
  Copies and pastes a version using the clipboard.
  Works only in IE as it's based on the default copy/paste commands in Sitecore.
  Available from the first level of the context menu in the Content Editor tree.


Copy Version To
...
  Shows a dialog in which you can choose a destination item for the version copy.
  Works in all browsers and is based on the default Copy To dialog.
  Available from the Copying item of the context menu in the Content Editor tree.


References
------------
Blog: http://www.partechit.nl/nl/blog/2014/02/copy-version-module  
GitHub: https://github.com/ParTech/Copy-Version


Installation
------------
The Sitecore package *\Release\ParTech.Modules.CopyVersion-1.0.0.zip* contains:
- Binary (release build).
- Configuration include file.
- Core items that add the commands to the context menu.

Use the Sitecore Installation Wizard to install the package.
After installation, the module will be immediately activated.


Release notes
-------------
*1.0.0*
- Initial release.


Author
------
This solution was brought to you and supported by Ruud van Falier, ParTech IT

Twitter: @BrruuD / @ParTechIT   
E-mail: ruud@partechit.nl   
Web: http://www.partechit.nl