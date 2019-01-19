# LottieSharp


| ![Logo](https://github.com/quicoli/LottieSharp/blob/master/Images/lottie_sharp-128.png?raw=true) |C# (WPF) port of Lottie (https://github.com/airbnb/lottie-android) based on .NET Framework 4.6.1 using sharpdx (http://sharpdx.org/)  |
|--|--|

**Usage**

```PM> Install-Package LottieSharp -Version 1.0.0```

- Install from nuget: [LottieSharp](https://www.nuget.org/packages/LottieSharp/);
- Import into your xaml the library

    xmlns:lottieSharp="clr-namespace:LottieSharp;assembly=LottieSharp"

- Now you can include the control in your layout
```xml
<lottieSharp:LottieAnimationView 
    x:Name="LottieAnimationView" 
    DefaultCacheStrategy="None" 
    FileName="Assets/Spider Loader.json" AutoPlay="True" 
    VerticalAlignment="Center" 
    HorizontalAlignment="Center"/>
```
The FileName property points to an After Effects animation file. You can write the filename in xaml like shown or bind to a property in your viewmodel.

The AutoPlay property indicates if the animation starts when the usercontrol is loaded.

There are many free animations at: [LottieFiles](https://www.lottiefiles.com/)
