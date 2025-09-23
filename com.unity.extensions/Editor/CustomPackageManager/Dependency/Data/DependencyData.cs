using System.Collections.Generic;

public static class DependencyData
{
    public static List<PackageData> dependencies = new List<PackageData>
    {
        new PackageData { name = "com.unity.2d.sprite", version = "1.0.0" },
        new PackageData { name = "com.cysharp.unitask", version = "2.5.10" },
        new PackageData { name = "extensions.unity.imageloader", version = "7.0.1" },
        new PackageData
        {
            name = "jillejr.newtonsoft.json-for-unity",
            version = "https://github.com/jilleJr/Newtonsoft.Json-for-Unity.git#upm",
        },
        new PackageData
        {
            name = "com.nobi.roundedcorners",
            version = "https://github.com/kirevdokimov/Unity-UI-Rounded-Corners.git",
        },
    };

    public static List<RegistryData> scopedRegistries = new List<RegistryData>
    {
        new RegistryData
        {
            name = "package.openupm.com",
            url = "https://package.openupm.com",
            scopes = new List<string>
            {
                "com.openupm",
                "com.cysharp.unitask",
                "extensions.unity.imageloader",
            },
        },
    };
}
