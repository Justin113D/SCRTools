# The language editor
The language editor is a tool to make translating for a team of translators as easy as possible! <br/>
Although it was made for the fangame '_Sonic Chronicles Remastered_', it can be used in **all** programs and games.

## How do i install the editor?
All required to do is to download the lates LE release on the release section of this github repo and extract the files to desired location. Then the .exe can be started without further ado. <br/>
Right now the language editor only exists for Windows

## How does the program work?
### Template/Format files
The first thing you are gonna need is a format file, which is an XML file with all specifications on which strings (texts) are gonna be ingame. <br/>
As of now, format files need to be written by hand, as there is no proper editor for it. (It's likely that one will be made in the future). <br/>
When starting the program, a default format file will be loaded, of which the file path can be specified in the settings. Per default, its in the "LanguageFiles" subfolder, named "format.xml".<br/>
The format files (can) also hold descriptions and default values for each field, to make translating easier, and can also categorize every field to keep a certain overview for the translators.

### Editing the strings/texts
Once a format file is loaded, the top level categories and strings will be loaded. Categories can be expanded and collapsed, which will then load the child- string and categories. <br/>
On the left side of a field is the name of the string/category, which cannot be edited, as those define how the string will be accessed ingame. On the right side is a description of the field, which is there to help the translator keep overview. <br/>
All string fields have a darker background to distinguish them from the category fields, and also have a separator line to distinct them from the strings above and below. <br/>
The dark box between the name and description is the value of the string, which is what will be displayed ingame. This textbox is what the translator is supposed to edit. <br/>
If a file of older version than the current format version is loaded, then the program will check which string fields have been created or updated since the language file was saved and mark those and all the categorys with a blue box to the left of the name. This way the translator has an easier time updating the language file.

Hint: Pressing Ctrl + R while editing a value resets it to the default value specified in the format file.

### Meta data
Below the Window frame are various informations about the current format file and the loaded file:
#### Author
Tells which translator created the file (has to be set by the author).
#### Language
Tells which language the file represents (also has to be set by the author).
#### File version
If no file is loaded, it will simply display "No file loaded". <br/>
If one is loaded, it will display the format version that the file was saved in.
#### Target name
Shows the target name of the format file. When loading a language file, it will compare its target name and that of the format file, and only attempt to load the language file if they match.
#### Format version
Shows the version of the loaded format

### File options and settings
Note: All file options can also be called via respective keybinding.
#### Load format
Lets you load another format file.
#### New file
Reverts all values and meta data to their default state.
#### Open
Opens a language file (requires both a .lang and a .lang.base file)
#### Save
Saves the current values to a .lang and a .lang.base file. Both of these files are required for loading, as the .lang holds all set strings and the .lang.base knows which line holds which value. <br/>
If no file has been opened or saved to before, the program will ask to pick a location to save to.
#### Save as
Lets you pick another location to save the changes to

### Settings
The program has only two different settings so far:
#### Default format path
Determines which format file will be opened upon loading the program
#### Theme
Changes the colors of the program interface. So far there is the dark and light theme

## How do the file formats work?
There are 3 different file types involved in this program: The format .xml, the .lang and the .lang.base files.

### The format file
The format file is an XML file that holds specifications for the game/program that it is made for. The file itself will not be used ingame, but is used to create the files used. <br/>
The structure is rather simple:

#### The 'Language' node
Everything is contained inside a `Language` node. The language node has no attributes. <br/>
The only requirements that the Language node needs to fulfill is to have a `Versions` subnode.  <br/>
It is also the main host for category and string nodes. <br/>
#### The 'Versions' and 'Version' nodes
The `Versions` subnode holds Version information about the format file and its previous versions, which is used to check which string nodes have been added or updated since a specific version (more on that further down). <br/>
It itself has no attributes either, but is a host for `Version` nodes, which also dont have any attributes. Instead the Version node holds a version (`Major.Minor.Build.Revision`) information. <br/>
The Versions should be in order, from oldest to newest version (The order doesnt actually matter but is adviced to be followed for clearer overview). <br/>
The newest version found in the Versions marks the format version.
#### The 'Cat' (Category) node
The category node itself plays no role in the language file that will be produced in the end. It simply holds more category- and string nodes, for a better overview. <br/>
The Node has 2 attributes: `Name` which is the name of the category node (required), and `Desc` (the description) which is optional and used to guide the translator.
#### The 'Str' (String) node 
The node of focus for the language file is the String file. It holds the value that will be accessed ingame. <br/>
The name has 3 attributes: `Name` which is the identifier by which its value will be accessed (required), `Desc` (the description) which is optional and used to guide the translator, and `vID` (version id), an integer, which tells the string in which version it was created or updated (required). <br/> 
If the version id is 0, it was created/last updated in the oldest version of the format. If the id is 1, and a language file with a version older than that of the version of index 1, then the string will be marked as "update required" in the program, which allows the translators to update the language files more easily.

### The .lang and .lang.base file
TODO

### Example:
if the format file is:
```
<Language>
	<Versions>
		<Version>1.0</Version>
	</Versions>
	<Cat Name="Container1">
		<Str vID="0" Name="MyString1">This is my first string</Str>
		<Str vID="0" Name="MyString3">This is my third string</Str>
	</Cat>
	<Cat Name="Container2">
		<Str vID="0" Name="MyString2">This is my second string</Str>
		<Str vID="0" Name="MyString4">This is my fourth string</Str>
	</Cat>
</Language>
```


Then the .lang file will look like this:
```
**TargetName**
**Version**
**Language**
**Author**
This is my first string
This is my second string
This is my third string
This is my fourth string
```


And the .lang.base file will look like this:
```
MyString1
MyString2
MyString3
MyString4
```

