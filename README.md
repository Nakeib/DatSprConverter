# opentibiaunity-datspr-converter

### Installing Packages
I've noticed that dlls are not uploaded (gitignore), consider getting them manually.

https://www.nuget.org/packages/Google.Protobuf  
https://www.nuget.org/packages/Newtonsoft.Json/

```cmd
Install-Package Google.Protobuf -Version 3.6.0
Install-Package Newtonsoft.Json -Version 11.0.2
```

### How to use?

Put tibia.dat, tibia.spr alongside within the application and start it
Once it finishes, appearancesXXX.dat, catalog-content.json and sprites folder will have been generated.

#### Available Options
| Option   |      Usage    |
|----------|:-------------:|
| --version* |  specify the version of the client |
| --dat* |    specify the relative location of the dat file   |
| --spr* | specify the relative location of the spr file |
| --alpha | specify whether to use transparency channel or not |
| --mappingFrom | specify whether to do id mapping from specific version |
| --mappingTo | specify whether to do id mapping to specific version |

> Note, options with `*` are required.

#### Example
``./opentibiaunity_datspr_converter --version=1098 --dat=Tibia.dat --spr=Tibia.spr``

``./opentibiaunity_datspr_converter --mappingFrom=870 --mappingTo=1098``
