// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// 拖拽服务
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DragDropService<T>
{
    /// <summary>
    /// 活动的Item
    /// </summary>
    public T? ActiveItem { get; set; }

    /// <summary>
    /// 悬停的项目
    /// </summary>
    public T? DragTargetItem { get; set; }

    /// <summary>
    /// 被拖拽的Items
    /// </summary>
    public List<T>? Items { get; set; }

    /// <summary>
    /// 之前的位置
    /// </summary>
    public int? OldIndex { get; set; }

    /// <summary>
    /// 通知刷新
    /// </summary>
    public EventHandler? StateHasChanged { get; set; }
}
