﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components;

/// <summary>
/// Step 组件类
/// </summary>
public partial class Steps
{
    /// <summary>
    /// 获得 组件样式字符串
    /// </summary>
    private string? ClassString => CssBuilder.Default("steps")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StepClassString(Step step) => CssBuilder.Default("step is-horizontal")
        .AddClass("is-flex", step.IsLast && !IsCenter)
        .AddClass("is-center", IsCenter || step.IsCenter)
        .Build();

    private static string? StepStyleString(Step step) => CssBuilder.Default("margin-right: 0px;")
        .AddClass($"flex-basis: {step.Space};", !string.IsNullOrEmpty(step.Space))
        .Build();

    private string? HeadClassString => CssBuilder.Default("step-head")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? LineStyleString => CssBuilder.Default()
        .AddClass("transition-delay: 150ms; border-width: 1px; width: 100%;", Status == StepStatus.Finish || Status == StepStatus.Success)
        .Build();

    private static string? StepIconClassString(Step step) => CssBuilder.Default("step-icon")
        .AddClass("is-text", step.IsIcon)
        .AddClass("is-icon", step.IsIcon)
        .Build();

    private string? IconClassString(Step step) => CssBuilder.Default("step-icon-inner")
        .AddClass(step.Icon, step.IsIcon || Status == StepStatus.Finish || Status == StepStatus.Success)
        .AddClass("fa fa-times", step.IsIcon || Status == StepStatus.Error)
        .AddClass("is-status", !step.IsIcon && (Status == StepStatus.Finish || Status == StepStatus.Success || Status == StepStatus.Error))
        .Build();

    private string? TitleClassString => CssBuilder.Default("step-title")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? DescClassString => CssBuilder.Default("step-description")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? StepString(Step step) => (Status == StepStatus.Process || Status == StepStatus.Wait) && !step.IsIcon ? (step.StepIndex + 1).ToString() : null;

    private Step? CurrentStep { get; set; }

    /// <summary>
    /// 获得/设置 步骤集合
    /// </summary>
    [Parameter]
    [NotNull]
    public List<Step>? Items { get; set; }

    /// <summary>
    /// 获得/设置 是否垂直渲染 默认 false 水平渲染
    /// </summary>
    [Parameter]
    public bool IsVertical { get; set; }

    /// <summary>
    /// 获得/设置 是否居中对齐
    /// </summary>
    [Parameter]
    public bool IsCenter { get; set; }

    /// <summary>
    /// 获得/设置 设置当前激活步骤
    /// </summary>
    [Parameter]
    public StepStatus Status { get; set; }

    /// <summary>
    /// 获得/设置 组件内容实例
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// 获得/设置 步骤组件状态改变时回调委托
    /// </summary>
    [Parameter]
    public Func<StepStatus, Task>? OnStatusChanged { get; set; }

    /// <summary>
    /// 获得/设置 步骤组模板
    /// </summary>
    [Parameter]
    public RenderFragment? StepTemplate { get; set; }

    private bool Init { get; set; }

    private string? StepHeaderClassString => CssBuilder.Default("steps-header")
        .AddClass("steps-horizontal", !IsVertical)
        .AddClass("steps-vertical", IsVertical)
        .Build();

    /// <summary>
    /// OnParametersSetAsync 方法
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        Items ??= new();
        CurrentStep = Items.Where(i => i.Status == StepStatus.Process).FirstOrDefault();
    }

    /// <summary>
    /// OnAfterRender
    /// </summary>
    /// <param name="firstRender"></param>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Init = true;
        }
    }

    /// <summary>
    /// Step 组件 OnInitialize 调用添加组件到集合中统一渲染
    /// </summary>
    /// <param name="step"></param>
    internal void AddItem(Step step)
    {
        Items.Add(step);
    }

    private string ParseSpace(string? space)
    {
        if (!string.IsNullOrEmpty(space) && !double.TryParse(space.TrimEnd('%'), out _))
        {
            space = null;
        }

        if (string.IsNullOrEmpty(space))
        {
            space = $"{Math.Round(100 * 1.0d / Math.Max(1, Items.Count() - 1), 2)}%";
        }

        return space;
    }
}
