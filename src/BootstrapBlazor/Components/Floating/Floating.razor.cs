// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components;

/// <summary>
/// 浮动层 组件
/// </summary>
public sealed partial class Floating : IAsyncDisposable
{
    private FloatingContext? Context { get; set; }

    private FloatingState State { get; set; } = new();

    private readonly Offset _offset = new();

    /// <summary>
    /// 获得/设置 浮动层的锚点元素ID
    /// </summary>
    [Parameter]
    public string? Anchor { get; init; }

    /// <summary>
    /// 获得/设置 浮动层的锚点元素选择器
    /// </summary>
    [Parameter]
    public string? Selecter { get; init; }

    /// <summary>
    /// 获得/设置 浮动层的锚点组件
    /// </summary>
    [Parameter]
    public IdComponentBase? Component { get; init; }

    /// <summary>
    /// 获得/设置 浮动层的锚点元素
    /// </summary>
    [Parameter]
    public ElementWrapper? Wrapper { get; init; }

    /// <summary>
    /// 获得/设置 浮动层的锚点位置
    /// </summary>
    [Parameter]
    public Offset? Offset { get; init; }

    /// <summary>
    /// 获得/设置 浮动层显示状态
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => State.Visible;
        set
        {
            if (State.Visible != value)
            {
                State.Visible = value;
                ShouldCompute = true;
            }

            //判断是不是虚拟元素，且位置有没有发生变化
            if (value && CheckOffsetChanged())
            {
                ShouldCompute = true;
            }
        }
    }

    /// <summary>
    /// 获得/设置 浮动层相对于它的锚点元素的位置
    /// </summary>
    [Parameter]
    public Placement Placement
    {
        get => State.Placement;
        set
        {
            if (State.Placement != value)
            {
                State.Placement = value;
                if (Visible)
                {
                    ShouldCompute = true;
                }
            }
        }
    }

    /// <summary>
    /// 获得/设置 浮动层显示时 使用的定位模式，默认绝对定位
    /// </summary>
    [Parameter]
    public Position Position
    {
        get => State.Position;
        init => State.Position = value;
    }

    /// <summary>
    /// 获得/设置 内层子组件内容 
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; init; }

    /// <summary>
    /// 获得/设置 浮动层渲染容器编号
    /// </summary>
    [Parameter]
    public string? ContainerId
    {
        get => State.ContainerId;
        init => State.ContainerId = value;
    }

    /// <summary>
    /// 获得/设置 当与锚点元素不在同一个剪辑上下文中时是否自动隐藏
    /// </summary>
    [Parameter]
    public bool AutoHide
    {
        get => State.AutoHide;
        init => State.AutoHide = value;
    }

    /// <summary>
    /// 获得/设置 是否自动更新位置，默认false
    /// </summary>
    [Parameter]
    public bool AutoUpdate
    {
        get => State.AutoUpdate;
        init => State.AutoUpdate = value;
    }

    /// <summary>
    /// 获得/设置 浮动元素和参考元素之间的距离。
    /// </summary>
    [Parameter]
    public int? AxisOffset
    {
        get => State.AxisOffset;
        init => State.AxisOffset = value;
    }

    /// <summary>
    /// 获得/设置 是否自动更改浮动元素的位置以使其处于可见状态。
    /// </summary>
    [Parameter]
    public bool UseFlip
    {
        get => State.UseFlip;
        init => State.UseFlip = value;
    }

    /// <summary>
    /// 获得/设置 配置需要填充的宽度，沿指定轴移动浮动元素以使其保持在视图中。
    /// </summary>
    [Parameter]
    public int? ShiftPadding
    {
        get => State.ShiftPadding;
        init => State.ShiftPadding = value;
    }

    /// <summary>
    /// 获得/设置 是否使用 提示箭头，浮动层必须预先设置为绝对定位
    /// </summary>
    [Parameter]
    public bool UseArrow
    {
        get => State.UseArrow;
        init => State.UseArrow = value;
    }

    /// <summary>
    /// 获得/设置 提示箭头的偏移量
    /// </summary>
    [Parameter]
    public int ArrowOffset
    {
        get => State.ArrowOffset;
        init => State.ArrowOffset = value;
    }

    /// <summary>
    /// 获得/设置 提示箭头的样式
    /// </summary>
    [Parameter]
    public string? ArrowStyleClass
    {
        get => State.ArrowStyleClass;
        init => State.ArrowStyleClass = value;
    }

    /// <summary>
    /// 获得/设置 组件样式类
    /// </summary>
    [Parameter]
    public string? @Class
    {
        get => State.Class;
        init => State.Class = value;
    }

    /// <summary>
    /// 获得/设置 组件样式
    /// </summary>
    [Parameter]
    public string? @Style
    {
        get => State.Style;
        init => State.Style = value;
    }

    /// <summary>
    /// 获得/设置 Floating 服务实例
    /// </summary>
    [Inject]
    [NotNull]
    private FloatingContainerService? Service { get; set; }

    /// <summary>
    /// 获得 当前浮动配置项，用于单元测试
    /// </summary>
    [ExcludeFromCodeCoverage]
    public string FloatingConfig => System.Text.Json.JsonSerializer.Serialize(State);

    /// <summary>
    /// 获得/设置 是否使用容器渲染
    /// </summary>
    private bool UseContainer { get; set; }

    /// <summary>
    /// 获得/设置 是否需要计算方位
    /// </summary>
    private bool ShouldCompute { get; set; }

    /// <summary>
    /// 获得/设置 是否已初始化渲染
    /// </summary>
    private bool FirstRendered { get; set; }

    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        CheckParameter();

        UseContainer = Service?.ContainerHasRegistered(ContainerId) ?? false;

        Context = new FloatingContext(Id!, ChildContent, State);
        if (UseContainer)
        {
            Service?.RegisterContext(Context);
        }
    }

    /// <summary>
    /// OnParametersSetAsync
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (ShouldCompute)
        {
            ShouldCompute = false;

            //缓存虚拟元素位置，供比对使用
            if (State.Category == FloatingCategory.Virtual)
            {
                _offset.Left = Offset!.Left;
                _offset.Top = Offset!.Top;
                _offset.Width = Offset!.Width;
                _offset.Height = Offset!.Height;
            }

            //支持重新渲染浮动内容
            Context!.ExecuteAfterRender(Toggle);
            if (FirstRendered)
            {
                Context!.UpdateFragment(Visible);
            }
        }
    }

    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            FirstRendered = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// ShouldRender
    /// </summary>
    /// <returns></returns>
    protected override bool ShouldRender() => false;

    /// <summary>
    /// 检查参数
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    private void CheckParameter()
    {
        if (Position != Position.Absolute && Position != Position.Fixed)
        {
            throw new NotSupportedException("Parameter Position：Only absolute and fixed are supported.");
        }

        if (!string.IsNullOrWhiteSpace(Anchor))
        {
            State.Category = FloatingCategory.Identifier;
        }
        else if (Wrapper != null)
        {
            State.Category = FloatingCategory.Element;
        }
        else if (Component != null)
        {
            State.Category = FloatingCategory.Component;
        }
        else if (!string.IsNullOrWhiteSpace(Selecter))
        {
            State.Category = FloatingCategory.Selecter;
        }
        else if (Offset != null)
        {
            State.Category = FloatingCategory.Virtual;
        }
        else
        {
            throw new ArgumentNullException(nameof(Anchor), "An anchor element must be set.");
        }
    }

    /// <summary>
    /// 检查锚点坐标是否发生变化
    /// </summary>
    private bool CheckOffsetChanged()
    {
        var isVitural = FirstRendered
            ? State.Category == FloatingCategory.Virtual
            : string.IsNullOrWhiteSpace(Anchor)
                && Wrapper == null
                && Component == null
                && string.IsNullOrWhiteSpace(Selecter)
                && Offset != null;

        if (isVitural)
        {
            return Offset!.Left != _offset.Left ||
                Offset!.Top != _offset.Top ||
                Offset!.Width != _offset.Width ||
                Offset!.Height != _offset.Height;
        }

        return false;
    }

    /// <summary>
    /// 切换浮动显示
    /// </summary>
    /// <returns></returns>
    private async ValueTask Toggle()
    {
        switch (State.Category)
        {
            case FloatingCategory.Identifier:
               await Compute(Anchor);
                break;
            case FloatingCategory.Element:
                await Compute(Wrapper?.Ref);
                break;
            case FloatingCategory.Component:
                await Compute(Component?.Id);
                break;
            case FloatingCategory.Selecter:
                await Compute(Selecter);
                break;
            case FloatingCategory.Virtual:
                await Compute(Offset);
                break;
        }
    }

    /// <summary>
    /// 计算浮动位置
    /// </summary>
    /// <typeparam name="TAnchor">元素类型</typeparam>
    /// <param name="anchor">锚点元素</param>
    /// <returns></returns>
    private async ValueTask Compute<TAnchor>(TAnchor anchor)
    {
        if (Context != null && anchor != null)
        {
            await JSRuntime.InvokeVoidAsync(Id, "bb_floating_toggle", anchor, Context.Element, State);
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    private async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            if (FirstRendered && AutoUpdate)
            {
                await JSRuntime.InvokeVoidAsync(Id, "bb_floating_cleanup");
            }
            if (UseContainer && Context != null)
            {
                Service?.UnregisterContext(Context);
            }
        }
    }

    /// <summary>
    /// Dispose 方法
    /// </summary>
    [ExcludeFromCodeCoverage]
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true);
        GC.SuppressFinalize(this);
    }
}
