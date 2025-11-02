# UI Navigation System 🔀
A lightweight and async-friendly UI navigation system for Unity.

Designed with mobile games in mind, UI Navigation System makes it easy to create, open, close, and sequence views (such as screens and popups) programmatically — all while keeping your game logic clean and testable.

```csharp
await navigationSystem.CreateView(ViewIds.MainMenu).Show().AwaitClose();
await navigationSystem.CreateView(ViewIds.Game).Show().AwaitClose();
```

It works under a single Canvas and integrates seamlessly with [UniTask](https://github.com/Cysharp/UniTask), with optional support for [DoTween](#dotween), [VContainer](#vcontainer) and [Tri-Inspector](#tri-inspector).

## Donations are appreciated! 💸
*Remember that are many ways to say thank you.*

If this timer has been helpful, remember to star the repository and consider buying me a coffee! 😀
<p>
<a href="https://www.buymeacoffee.com/ebatlleclavero" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-blue.png" alt="Buy Me A Coffee" width="144.6" height="34"></a>
</p>

If you like my general work and contributions consider [sponsoring me on Github](https://github.com/sponsors/EricBatlle).

But if you just want to donate straightforward, I also have [PayPal.me](https://paypal.me/EricBatlleClavero?locale.x=es_ES).

## ✨ Key Features

- 📱 Mobile-oriented
- ⚡ Async UI flow with [UniTask](https://github.com/Cysharp/UniTask)
- 🧩 Plug-and-play and [extendable design](#you-can-create-your-own-view-capabilities) view composition through interfaces
- 🎨 [Animated transitions](#openclose-animations) for your views
- 🎬 Optional Built-in support for animated transitions with [DoTween](#dotween)
- 📦 Optional Built-in support for DI with [VContainer](#vcontainer)
- 🎨 Optional Built-in support for [Tri-Inspector](#tri-inspector)

## 🔗Dependencies
- This package depends on [UniTask](https://github.com/Cysharp/UniTask) to create 0 allocations when creating Tasks.
  - If you don't know how to work with it, please check out respective docs first.

## 🛠️ How to Install 
- Update scope registry by adding this to your project manifest.json:
  - This will allow UniTask to be installed automatically through OpenUPM
```json
"scopedRegistries": [
    {
        "name": "OpenUPM",
        "url": "https://package.openupm.com",
        "scopes": [
            "com.cysharp"
        ]
    }
]
```
- Install UI Navigation System using the Git URL: https://github.com/EricBatlle/NavigationSystem
  - Remember that a scripting define symbol ``UNITASK_DOTWEEN_SUPPORT`` will be added automatically if [DoTween](#dotween) is detected.

## ⚙️ How to Use
- Create at least one ``ViewsContainer`` ScriptableObject, right click -> Create/ScriptableObject/ViewsContainer
- Create one class to hold your view Ids and add to it the ``[ViewsContainer]`` attribute:
```csharp
[ViewIdsContainer]
public static class ViewIds
{
    public const string MainMenu = "MainMenu";
}
```
- Instantiate ``NavigationSystem``
  - Notice that you can create your own implementation of ``IViewsFactory``, for example the one used to work with VContainer [explained here](#VContainer).
```csharp
IViewsFactory viewsFactory = new DefaultViewsFactory();
navigationSystem = new NavigationSystem.NavigationSystem(viewsContainer, viewsFactory, rootCanvasTransform);
```
- Create the prefab you want to use as View
  - By default, UI Navigation System will auto-attach a ``DefaultView`` script unless you [add your own](#how-to-create-your-own-views)
- Add one entry inside ViewsContainer by adding both Id and the associated prefab (your view prefab)

Now you are ready to consume this navigation system inside your services.
```csharp
public class FlowService
{
    private readonly NavigationSystem.NavigationSystem navigationSystem;

    public SomeService(NavigationSystem.NavigationSystem navigationSystem)
    {
        this.navigationSystem = navigationSystem;
    }

    public async UniTask Execute()
    {
        // Simplest case scenario
        await navigationSystem.CreateView(ViewIds.MainMenu).Show();
    }
    
    // If we want to create a sequence of views that will open when the previous one is closed, we can do the following:
    public async UniTask ExecuteSequentially()
    {
        await navigationSystem.CreateView(ViewIds.MainMenu).Show().AwaitClose();
        await navigationSystem.CreateView(ViewIds.Game).Show(); // Game will not be shown until MainMenu is closed
        // Do something else while Game is opened
    }
}
```

## 🔨 How to create your own views
If you don't attach any script to your view prefab that inherits from ``IView``, this system will add a ``DefaultView`` script, but you can add more advanced behaviours to it.

Since your prefabs are ``GameObjects``, everything you can add to them are Components (``MonoBehaviours``), for that you can create your own ``MonoBehaviour`` Component and inherit from any of those 2 classes:
- ``BaseView``
- ``BaseAnimatedView``
  - Same as ``BaseView`` but with animated Open and Close transitions that you can customize, more info about it [here](#Transition-Animations).
- ``DoTweenAnimatedView``
  - Only available if you have DoTween installed, more info about it [here](#dotween)
  
This component is designed to be your View entry point, where you can add extended behaviours.
```csharp
public class LoseView : BaseView
{
      [Header(nameof(LoseView))]
      [SerializeField]
      private Button closeButton;
      
      private void Awake()
      {
          closeButton.onClick.AddListener(() => Close());
      }
}
```
## 📦 Passing Data to Views
Imagine that you want to instantiate your prefab with some dynamic data in it.

In this case our view component will extend ``IViewWithData<TViewData>``, like:
```csharp
public class LoseView : BaseView, IViewWithData<float>
{
    [Header(nameof(LoseView))]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    
    private float userScore;

    public void SetData(float userScore)
    {
        scoreText.text = userScore.ToString();
    }
}
```
``SetData`` will be called before showing the view, so when the view is visible for the user, everything will be set in time.
This is how to use it:

```csharp
public async UniTask Execute()
{
    await navigationSystem.CreateView(ViewIds.Lose)
        .WithData(10)
        .Show();
}
```

## 🔁 Getting Results from Views
Imagine that you have a confirmation view, and you want to write a Flow class for it.
There are multiple ways to approach this scenario, but for the sake of the explanation let's extend from ``IViewWithResult<TViewResult>``, like:
```csharp
public class LoseView : BaseView,  IViewWithResult<bool>
{
    [Header(nameof(LoseView))]
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button declineButton;

    public UniTask<bool> AwaitResult => resultTcs.Task;
    private readonly UniTaskCompletionSource<bool> resultTcs = new();
    
    private void Awake()
    {
        confirmButton.onClick.AddListener(() => resultTcs.TrySetResult(true));
        declineButton.onClick.AddListener(() => resultTcs.TrySetResult(false));
    }

    public override UniTask Close(bool immediate = false)
    {
        if (resultTcs.Task.Status == UniTaskStatus.Pending)
        {
            resultTcs.TrySetResult(false);
        }
        return base.Close(immediate);
    }
}
```
This allows us to call views within the following ways:
```csharp
public async UniTask Execute()
{
    // This will return the result once the view is closed
    var isConfirmed = await navigationSystem.CreateView(ViewIds.Lose)
        .Show()
        .AwaitCloseResult<bool>();
}

public async UniTask ExecuteSequentially()
{
    // Here, however, we will open a new window once confirmed, but we will not close it until the NEXT view is closed.
    var (viewHandle,isConfirmed) = await navigationSystem.CreateView(ViewIds.Lose)
            .Show()
            .AwaitResult<bool>();
    await navigationSystem.CreateView(ViewIds.MainMenu).Show().AwaitClose();
    await viewHandle.AwaitClose();
}
```
## 🧬 Everything at once
Since [those behaviours](#-passing-data-to-views) are added through composition, not inheritance, you can combine them, and for example create a view that accepts a [dynamic data](#-passing-data-to-views), and also [returns a value](#-getting-results-from-views).
```csharp
public class LoseView : BaseView, IViewWithData<float>, IViewWithResult<bool>
{
      [Header(nameof(LoseView))]
      [SerializeField]
      private Button confirmButton;
      [SerializeField]
      private Button declineButton;
      [SerializeField]
      private TextMeshProUGUI scoreText;
      
      private float userScore;
      public UniTask<bool> AwaitResult => resultTcs.Task;
      private readonly UniTaskCompletionSource<bool> resultTcs = new();
      
      private void Awake()
      {
          confirmButton.onClick.AddListener(() => resultTcs.TrySetResult(true));
          declineButton.onClick.AddListener(() => resultTcs.TrySetResult(false));
      }
      
      public void SetData(float userScore)
      {
          scoreText.text = userScore.ToString();
      }
      
      public override UniTask Close(bool immediate = false)
      {
          if (resultTcs.Task.Status == UniTaskStatus.Pending)
          {
              resultTcs.TrySetResult(false);
          }
          return base.Close(immediate);
      }
}
```
and use it like:
```csharp
public class FlowService
{
    private readonly NavigationSystem.NavigationSystem navigationSystem;

    public SomeService(NavigationSystem.NavigationSystem navigationSystem)
    {
        this.navigationSystem = navigationSystem;
    }

    public async UniTask Execute()
    {
        // This will open the view with a value of 10, AND return the result once the view is closed
        var isConfirmed = await navigationSystem.CreateView(ViewIds.Lose)
            .WithData(10)
            .Show()
            .AwaitCloseResult<bool>();
    }
}
```

## 🧩 You can create your own view capabilities
Feel free to create and share other behaviours you think that can be useful to add to this library.

The easiest way to create them is to create a contract that inherits from ``IView`` and then an extension method for ``ViewHandle`` or ``UniTask<ViewHandle>`` like this one:
```csharp
public interface IViewWithData<TViewData> : IView
{
    public void SetData(TViewData viewData);
}

public static ViewHandle WithData<TViewData>(this ViewHandle screen, TViewData data)
{
      screen.Require<IViewWithData<TViewData>>().SetData(data);
      return screen;
}
```

## 🎬 Open/Close Animations
As mentioned before, ``BaseAnimatedView`` is just a shortcut to create a ``BaseView`` that automatically includes an Open and Close transition components to your prefab (due to ``RequireComponent`` tag).

That means that ``BaseAnimatedView == BaseView + (IOpenViewTransition + ICloseViewTransition)``.

But you don't need to inherit from ``BaseAnimatedView`` to have animated views, you can create your own animated view from scratch:

Create your view script that inherits from ``BaseView``, and add the contracts for Open and/or Close transitioning the view:
- ``IOpenViewTransition``
- ``ICloseViewTransition``

By default, this package includes one default Open and Close transition animation, so you can modify its values from Unity Editor, or you can create your own different ones for different type of views.

Simply create a new script that inherits from ``ViewTransition`` and implement ``Animate`` method.

Add the required fields to your View Script to attach the previous created ViewTransitions MonoBehaviours.

Note: View transitions are build as MonoBehaviours to make it easy to tweak from the editor, but since all that matters are the contracts, you can create animations as scriptable objects or plain c# code if you prefer.

# 🔌Optional Integrations

All this features are automatically enabled by default if you are using those third party plugins, and if you are not, they will be unlocked automatically once you add them.

## 📦 [VContainer](https://github.com/hadashiA/VContainer)
If you are using [VContainer](https://github.com/hadashiA/VContainer), in your project, this UINavigationSystem offers some utilities to bind the navigation system easily as follow:
- Bind NavigationSystem into your ``LifetimeScope``.
  - This requires a reference to which transform you want your views to be instantiated, and the reference to the ViewContainer to use in this context.
```csharp
public class BootstrapLifeTime : LifetimeScope
{
    [SerializeField] private ViewsContainer viewsContainer;
    [SerializeField] private Transform rootCanvasTransform;

    protected override void Configure(IContainerBuilder builder)
    {
        new NavigationSystemInstaller(rootCanvasTransform, viewsContainer).Install(builder);
    }
}
```
Notice that the ``IViewFactory`` is not the default one as explained at the beginning, but instead it uses a ``VContainerViewsFactory`` that will take care for you the binding between lifetimescopes and contexts.

## 🎬 [DoTween](https://github.com/Demigiant/dotween)
If you are using [DoTween](https://github.com/Demigiant/dotween) in your project, this UINavigationSystem offers some extra utilities like:
- ``DoTweenOpenViewTransition``
- ``DoTweenCloseViewTransition``

They act the same as [Open/Close Animations](#openclose-animations), explained here, but using the power and flexibility of [DoTween](https://github.com/Demigiant/dotween).

- ``DoTweenBaseAnimatedView`` -> Same as ``BaseAnimatedView`` but using explained previous Tweenable transitions

## 🎨 [Tri-Inspector](https://github.com/codewriter-packages/Tri-Inspector)
If you are using [Tri-Inspector](https://github.com/codewriter-packages/Tri-Inspector) in your project, this UINavigationSystem is fully compatible with it.

And it will properly serialize ViewId even inside [Tri-Inspector](https://github.com/codewriter-packages/Tri-Inspector) contexts like:
```csharp
[Button]
public void OpenView(ViewId viewId)
{
      navigationSystem.CreateView(viewId);
}
```