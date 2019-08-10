# SteganoHydra v.1.0.1

**SteganoHydra** is a lightweight tool for performing Steganography operations combined with Cryptography for data security of the payload. This tool supports embedding text string as well as multiple files into a single PNG image. The security itself uses AES-256 with 20,000 iterations for the user password. 
**SteganoHydra** supports 3 operations. They are message (string) embedding, single file embedding and multiple files embedding (must specify the name of the target folder).

#### Switch Descriptions

| Command | Switch | Description |
| --- | --- | --- |
| -em | | Embed data/payload into selected PNG image |
| | -i "input_image.png" | Input file. This is target PNG image you wish the payload to be embedded to|
| | -o "output_image.png" | Output file. This is the output file after encoding/embedding process.|
| | -p "password" | Password to protect the payload data. Must be at least 4 chars length|
| | -s "text message" | Embed string/text message |
| | -f "file.pdf" | Embed single file. File must exist otherwise will be error|
| | -d "folder_location" | Embed multiple files inside selected folder location. Folder must exist and must contain at least one file|
|-ex| | Extract payload inside stegafied PNG image |
| | -i "input_image.png" | Input file. This is target PNG image you wish the payload to be extracted from|
| | -p "password" | Password to extract the payload. Must be at least 4 chars length and must match with the defined password inside the stegafied image|
| | -s | Extract string/text message |
| | -d "folder_location" | Extract files from input image/stegafied PNG image|


OK, here are a couple of examples of SteganoHydra Operations.

### Embedding a text message into a single PNG image.

This command embed any kind of string you put inside double quotes "<string>" to any PNG image you selected. Like any other commands, you need to specify a password to protect the payload data.

```powershell
shydra.exe -em -i "G:\Samples\input\picture1.PNG" -o "G:\Samples\output\picture1_stegafied.PNG" -s "Hello World. I am MIRZA GHULAM RASYID" -p future
```
![Command 1](https://raw.githubusercontent.com/mirzaevolution/SHydra.Console/master/resources/images/text-embed.PNG)

**Image Output Result:**

![Output 1](https://github.com/mirzaevolution/SHydra.Console/blob/master/resources/images/picture1_stegafied.PNG?raw=true)

**Extracted Message:**
```powershell
shydra -ex -i "G:\samples\output\picture1_stegafied.png" -s -p future
```

```
Hello World. I am MIRZA GHULAM RASYID
```

### Embedding a single file

This command only embeds a single file into a selected PNG image. You freely specify the extension (no limit). If you specify the attachment file that doesn't exist, it will raise an exception. 

```powershell
shydra.exe -em -i "G:\Samples\Input\picture2.png" -o "G:\Samples\output\picture2_stegafied.png" -f "G:\Samples\Input\watch.exe" -p PASSWORD123
```

![Command 2](https://raw.githubusercontent.com/mirzaevolution/SHydra.Console/master/resources/images/file-embed.PNG)

**Image Output Result:**

![Output 2](https://github.com/mirzaevolution/SHydra.Console/blob/master/resources/images/picture2_stegafied.png?raw=true)

**Extract Embedded File:**
```powershell
shydra -ex -i "G:\Samples\output\picture2_stegafied.png" -d "G:\Samples\Extract" -p PASSWORD123
```

File ***watch.exe*** inside ***picture2_stegafied.png*** will be extracted to **G:\Samples\Extract\watch.exe."**


### Embedding multiple files inside a folder

To embed multiple files, you must specify the folder location. Engine will scan top directory files inside specified folder location and embed them into PNG image you've specified. 

```powershell
shydra.exe -em -i "G:\Samples\Input\picture3.png" -o "G:\Samples\Output\picture3_stegafied.png" -d "G:\Samples\SourceFiles" -p PWD!@#$!@
```

![Command 3](https://raw.githubusercontent.com/mirzaevolution/SHydra.Console/master/resources/images/files-embed.PNG)

**Image Output Result:**

![Output 3](https://github.com/mirzaevolution/SHydra.Console/blob/master/resources/images/picture3_stegafied.png?raw=true)

**Extract Embedded Files:**
```powershell
shydra -ex -i "G:\Samples\Output\picture3_stegafied.png" -d "G:\extract" -p PWD!@#$!@
```
![Extracted-Files](https://github.com/mirzaevolution/SHydra.Console/blob/master/resources/images/source-files2.PNG?raw=true)


### Created by: [Mirza Ghulam Rasyid](https://linkedin.com/in/mirzaghulamrasyid/)
