using BluePope.HomeWeb.Hubs.Chat;
using BluePope.HomeWeb.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Controllers.ViewComponents
{
    public class LeftMenu : ViewComponent
    {
        //static IHubContext<ChatHub> ChatHub;

        public LeftMenu()//IHubContext<ChatHub> chatHub
        {
            //ChatHub = chatHub;
        }
        
        public IViewComponentResult Invoke()
        {
            //singleton 형태로 바꿔서 내용을 좀 줄일수 있을 것 같은데

            var controller = ViewContext.RouteData.Values["Controller"] as string;
            var action = ViewContext.RouteData.Values["Action"] as string;

            var list = new List<MLeftFolder>();
            list.Add(MLeftFolder.Create((setting) => {
                setting.ControllerName = "realtime";
                setting.Name = "RealTime";

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "chat";
                    menu.Name = "채팅";
                    //menu.NotiCount = 1; signalr 은 hub clients count 가 없음 별도로 구현해야함
                }));
            }));

            list.Add(MLeftFolder.Create((setting) => {
                setting.ControllerName = "clientside";
                setting.Name = "Client Side";

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "vue";
                    menu.Name = "VueJs";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "ajaxTest";
                    menu.Name = "Ajax Test";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "axiosTest";
                    menu.Name = "Axios Test";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "DragAndDropMultipleupload";
                    menu.Name = "드래그앤드롭";
                }));
            }));

            list.Add(MLeftFolder.Create((setting) => {
                setting.ControllerName = "board";
                setting.Name = "게시판";

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "list";
                    menu.Name = "리스트";
                }));
            }));

            list.Add(MLeftFolder.Create((setting) => {
                setting.ControllerName = "Lab";
                setting.Name = "실험실";

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "BarCodeGenerator";
                    menu.Name = "바코드 생성";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "DeliveryCheck";
                    menu.Name = "택배 조회";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "WebFileDownload";
                    menu.Name = "웹 파일 다운로드 테스트";
                }));

                setting.SubMenuList.Add(MLeftMenu.Create((menu) => {
                    menu.ActionName = "DbTableList";
                    menu.Name = "MySql 테이블 모델 만들기";
                }));
            }));

            list.FirstOrDefault(p => p.ControllerName.ToLower() == controller.ToLower())?.SubMenuList.FirstOrDefault(p => p.ActionName.ToLower() == action.ToLower())?.setActive(true);

            return View("Default", list);
        }

    }
}
