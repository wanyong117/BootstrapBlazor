﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BootstrapBlazor.Shared.Samples
{
    /// <summary>
    /// 消息分发示例
    /// </summary>
    public partial class Dispatchs : IDisposable
    {
        [Inject]
        [NotNull]
        private IDispatchService<MessageItem>? DispatchService { get; set; }

        [Inject]
        [NotNull]
        private ToastService? Toast { get; set; }

        [Inject]
        [NotNull]
        private WebClientService? ClientService { get; set; }

        [Inject]
        [NotNull]
        private IIPLocatorProvider? IPLocator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            DispatchService.Subscribe(Dispatch);
        }

        private async Task Dispatch(DispatchEntry<MessageItem> entry)
        {
            if (entry.Entry != null)
            {
                // 获得当前用户 IP 地址
                if (await ClientService.RetrieveRemoteInfo() && ClientService.Ip != null)
                {
                    var location = await IPLocator.Locate(ClientService.Ip);
                    await Toast.Show(new ToastOption()
                    {
                        Title = "Dispatch 服务测试",
                        Content = $"{entry.Entry.Message} 来自 {location}",
                        Category = ToastCategory.Information,
                        Delay = 30 * 1000,
                        ForceDelay = true
                    });
                }
            }
        }

        private async Task OnDispatch()
        {
            DispatchService.Dispatch(new DispatchEntry<MessageItem>()
            {
                Name = nameof(MessageItem),
                Entry = new MessageItem()
                {
                    Message = $"{DateTime.Now:HH:mm:ss} 测试通知消息"
                }
            });
            await Task.Delay(30 * 1000);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DispatchService.UnSubscribe(Dispatch);
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
}