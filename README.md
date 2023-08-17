# LottieSharp

| ![Logo](https://raw.githubusercontent.com/ascora/LottieSharp/master/Images/lottie_sharp-128.png) | Play [LottieFiles](https://lottiefiles.com/) in your WPF application  |
|--|--|

LottieSharp is built for WPF applications only. It targets .NET 6 and .NET Framework 4.7  and is built using [SkiaSharp](https://github.com/mono/SkiaSharp) and [Skottie](https://skia.org/docs/user/modules/skottie/).

### What can I do with LottieSharp?
You can load [lottie animations](https://lottiefiles.com/) and play them in your applications, creating beautiful UIs.

![demo in action](https://raw.githubusercontent.com/ascora/LottieSharp/develop/Images/demo.gif "Demo in Action")

PS.: Screen cast by: http://recordit.co/ 

### How to start?


Add LottieSharp to your application:

```PM> Install-Package LottieSharp -Version 2.3.0```

Reference LottieSharp in your XAML Window/Page/UserControl:

```xmlns:lottie="clr-namespace:LottieSharp.WPF;assembly=LottieSharp"```

Add a LottieAnimationView control. Set properties as you wish:
```
<lottie:LottieAnimationView
    Width="200"
    Height="300"
    HorizontalAlignment="Center"
    VerticalAlignment="Center"
    AutoPlay="True"
    FileName="{Binding Path=SelectedAsset.FilePath}"
    RepeatCount="-1" />
```

With `images/my-resource-animation.json` Resource in the application project file:
```
<lottie:LottieAnimationView
    Width="200"
    Height="300"
    HorizontalAlignment="Center"
    VerticalAlignment="Center"
    AutoPlay="True"
    ResourcePath="pack://application:,,,/images/my-resource-animation.json"
    RepeatCount="-1" />
```

When a lottie animation is small and you need it bigger, there's no need to edit the lottie file.
Now we apply a scale to it.
```
<lottie:LottieAnimationView
    Width="200"
    Height="300"
    HorizontalAlignment="Center"
    VerticalAlignment="Center"
    AutoPlay="True"
    ResourcePath="pack://application:,,,/images/my-resource-animation.json"
    RepeatCount="-1">
    <lottie:LottieAnimationView.AnimationScale>
          <transforms:CenterTransform ScaleX="1.5" ScaleY="1.5" />
    </lottie:LottieAnimationView.AnimationScale>
</lottie:LottieAnimationView>
```

CenterTransform scales the animation within its center automatically.
If you need a different position use the AnimationTransformBase.
```
<lottie:LottieAnimationView
    Width="200"
    Height="300"
    HorizontalAlignment="Center"
    VerticalAlignment="Center"
    AutoPlay="True"
    ResourcePath="pack://application:,,,/images/my-resource-animation.json"
    RepeatCount="-1">
    <lottie:LottieAnimationView.AnimationScale>
          <transforms:AnimationTransformBase
               CenterX="0"
               CenterY="1"
               ScaleX="2"
               ScaleY="2" />
    </lottie:LottieAnimationView.AnimationScale>
</lottie:LottieAnimationView>
```

### Version 2.3.0
Adds support to .NET Framework 4.7.

### Version 2.2.0
Adds support for scaling the animation.
Adds support to AnyCPU.
Updated to latest SkiaSharp references.

### Version 2.1.0
Adds support for loading a Resource stream with a `pack://application` URI. Using both `FileName` and `ResourcePath` properties is ambigous.

### Version 2.0.1
Fixed issue #57.
Fixed issue with animation details not being displayed in databinding.


### Properties, Methods and Events

| Properties     | Values                | Description |
| --- | --- | --- |
| AutoPlay       | True, False           | When true, the animation file is automatically played and it is loaded |
| FileName       | string                | Path to the Lottie file. This property can be used in databind (see demo app) |
| ResourcePath   | string                | Resource path to the Lottie file. This property can be used in databind (see demo app splash screen) |
| RepeatCount    | -1..N                 | How many times the animation will repeat after once played. The default is 0, meaning it doesn't repeat. -1 means it repeats forever. |
| IsPlaying      | True, False           | Represents the current aninaation status. |
| AnimationScale |AnimationTransformBase | Applies a custom scale to the loaded lottie animation. See CenterTransform above |


| Events | Description |
| --- | --- |
| EventHandler OnStop | It's triggered when animation stops, however if RepeatCount is forever this event isn't triggered. |


| Methods | Description |
| --- | --- |
| PlayAnimation() | Starts the animation |
| StopAnimation() | Stops the animation |

### Next steps
This is the first release with basic features but very functional. For next releases I want to:
- Improve player mechanism
- Add reverse mode


### Questions?
>### Why Lottie?
>Lottie enables us to easily include beautiful and performant vector animations in applications. Since Lottie animations are exported as JSON files, file sizes remain >small and animations can easily be resized and looped with without losing quality. So, no more heavy videos or gifs!

>### Where do I find lottie animations?
>There are many **FREE** files you can use. Visit [LottieFiles](https://lottiefiles.com/) website, there are a huge community for lottie!

>### How do I create my own animations?
> 1. Adober effects, see [this post](https://uxdesign.cc/creating-lottie-animations-with-after-effects-e5124feb8a9c)
> 2. [Framer](https://www.framer.com/plugins/lottie/)
> 3. And others... see [this post](https://github.com/LottieFiles/awesome-lottie)


### Disclaimer
> Version 1.1.3 is archived in master branch and is no longer maintained. 
> All new implementations are done in develop branch.
