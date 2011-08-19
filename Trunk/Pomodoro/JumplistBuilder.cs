using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using Pomodoro.Properties;
using JumpList = System.Windows.Shell.JumpList;

namespace Pomodoro
{
    public class JumplistBuilder
    {
       

        public static void CreateJumpList()
        {
            var jumpList = new JumpList();
            JumpList.SetJumpList(Application.Current, jumpList);

            var restartTask = new JumpTask
                                  {
                                      Title = Resources.MainWindow_CreateJumpList_Start_New_Session,
                                      Description = Resources.MainWindow_CreateJumpList_Starts_a_new_Pomodoro_session_,
                                      ApplicationPath = Assembly.GetEntryAssembly().Location,
                                      Arguments = "/restart",
                                      IconResourcePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Pomodoro.Icons.dll",
                                      IconResourceIndex = 0
                                  };
            jumpList.JumpItems.Add(restartTask);
            /*
            var startTask = new JumpTask
                                {
                                    Title = Resources.MainWindow_CreateJumpList_Start_Timer,
                                    Description = Resources.MainWindow_CreateJumpList_Starts_the_timer_if_it_is_stopped_,
                                    ApplicationPath = Assembly.GetEntryAssembly().Location,
                                    Arguments = "/start",
                                    IconResourcePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Pomodoro.Icons.dll",
                                    IconResourceIndex = 0
                                };
            jumpList.JumpItems.Add(startTask);
            var stopTask = new JumpTask
                               {
                                   Title = Resources.MainWindow_CreateJumpList_Stop_Timer,
                                   Description = Resources.MainWindow_CreateJumpList_Stops_the_timer_if_it_is_started_,
                                   ApplicationPath = Assembly.GetEntryAssembly().Location,
                                   Arguments = "/stop",
                                   IconResourcePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Pomodoro.Icons.dll",
                                   IconResourceIndex = 0
                               };
            jumpList.JumpItems.Add(stopTask);
            var stopRingingTask = new JumpTask
                                      {
                                          Title = Resources.MainWindow_CreateJumpList_Stop_Ringing,
                                          Description = Resources.MainWindow_CreateJumpList_Mutes_a_ringing_timer_and_prepares_for_a_new_session,
                                          ApplicationPath = Assembly.GetEntryAssembly().Location,
                                          Arguments = "/stopringing",
                                          IconResourcePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Pomodoro.Icons.dll",
                                          IconResourceIndex = 0
                                      };
            jumpList.JumpItems.Add(stopRingingTask);
             */

            jumpList.Apply();
        }
    }
}