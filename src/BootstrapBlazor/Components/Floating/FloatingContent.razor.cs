// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components;

/// <summary>
/// 浮动层 组件
/// </summary>
public partial class FloatingContent : IDisposable
{
    private string? ClassString => CssBuilder.Default()
        .AddClass(Context?.State.Class)
        .Build();

    private string? StyleClassString => CssBuilder.Default()
        .AddClass(Context?.State.Style)
        .AddClass($"position:{Context?.State.Position_};")
        .AddClass($"display:none;")
        .Build();

    private string? ArrowStyleClassString => CssBuilder.Default()
        .AddClass($"position:{Context?.State.Position_};")
        .Build();

    /// <summary>
    /// 获得/设置 浮动层显示状态，默认隐藏
    /// </summary>
    [Parameter]
    public FloatingContext? Context { get; set; }

    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Context != null)
        {
            Context.FragmentChanged += OnFragmentChanged;
        }
    }


    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        //执行延迟的方法
        if (Context?.ExecuteAfterRenderQueue?.Count > 0)
        {
            while (Context.ExecuteAfterRenderQueue.Count > 0)
            {
                await Context.ExecuteAfterRenderQueue.Dequeue()();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnFragmentChanged(object? sender, bool e) => await InvokeAsync(StateHasChanged);

    /// <summary>
    /// Dispose 方法
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
        if (disposing && Context != null)
        {
            Context.FragmentChanged -= OnFragmentChanged;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
