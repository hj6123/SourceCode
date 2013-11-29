using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;
namespace SystemHook
{

    public class HookBarCode
    {
        public delegate void HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void GetBarCodeDelegate(string _BarCode);
        private event GetBarCodeDelegate GetBarCodeEvent;
        private HookProc KeyboardHookProcedure;
        private IntPtr  m_lHook=IntPtr.Zero ;
        private  StringBuilder ScanCode;   //ɨ���� 
        private  int TickCount;            //ɨ��ʱ��

        #region �����͹رչ���
        /// <summary>
        /// ��ϵͳ���ӣ���ʼ����ɨ��ǹ
        /// </summary>
        public void StartSystemHook()
        {
            if (IntPtr.Zero == m_lHook)
            {
                KeyboardHookProcedure += CallHookProc; 
                m_lHook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            }
        }

        /// <summary>
        /// ���̹߳��ӣ���ʼ����ɨ��ǹ
        /// </summary>
        public void StartThreadHook(int threadID)
        {
            if (IntPtr.Zero == m_lHook)
            {
                KeyboardHookProcedure += CallHookProc;
                m_lHook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, KeyboardHookProcedure,
                    IntPtr.Zero, threadID);
                
            }
        }

        /// <summary>
        /// �رռ���
        /// </summary>
        public void StopHook()
        {
            if (IntPtr.Zero != m_lHook)
                UnhookWindowsHookEx(m_lHook);
        }
        #endregion

        /// <summary>
        /// ���ɨ��ǹ¼�봦���¼�
        /// </summary>
        /// <param name="GetBarCodeMethod"></param>
        public void Add(GetBarCodeDelegate GetBarCodeMethod)
        {
            GetBarCodeEvent += new GetBarCodeDelegate(GetBarCodeMethod);
        }

        //�ص�����
        private void CallHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            /* 
                KeyDown			= 0x0100,
	            KeyUp			= 0x0101,
	            SystemKeyDown	= 0x0104,
	            SystemKeyUp		= 0x0105
            */
            if (GetBarCodeEvent != null && nCode >= 0 && (int)wParam == 0x100)
            {
                HookEventMSG m = (HookEventMSG)Marshal.PtrToStructure(lParam, typeof(HookEventMSG));
                
                if (m.vkCode >= 48 && m.vkCode <= 57)
                {
                    //���ɨ���������
                    AddText ( (m.vkCode - 48).ToString());
                }
                else if(m.vkCode>=65 && m.vkCode <=90)
                { 
                    //ɨ��Ϊ��ĸ
                    //AddText(ConvertKeyCode((VirtualKeys)m.vkCode).ToString());
                    AddText(((char)m.vkCode).ToString ());
                }
                else if (13 == m.vkCode)
                { 
                    //�������Ϊ�س�
                    if (IsAdd() && ScanCode.Length > 3)
                    {
                        GetBarCodeEvent(ScanCode.ToString ());
                    }
                    
                }
            }
            //CallNextHookEx(m_lHook, nCode, wParam, lParam);
        }

        #region �Զ���˽�з���
        //��������ַ���ӵ�ɨ����ĩβ
        private void AddText(string text)
        {
            if (!IsAdd ())
            {
                ScanCode = new StringBuilder();
            }
            ScanCode.Append(text);
        }

        private bool IsAdd()
        {
            if (Math.Abs(System.Environment.TickCount - TickCount) < 50)
            {
                TickCount =System.Environment.TickCount;
                return true;
            }
            else
            {
                TickCount = System.Environment.TickCount;
                return false;
            }
        }
        #endregion

        #region ���Ӷ���
        // ************************************************************************
        // Win32: SetWindowsHookEx()
        [DllImport("user32.dll")]
        protected static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);
        // ************************************************************************

        // ************************************************************************
        // Win32: UnhookWindowsHookEx()
        [DllImport("user32.dll")]
        protected static extern int UnhookWindowsHookEx(IntPtr hhook);
        // ************************************************************************

        // ************************************************************************
        // Win32: CallNextHookEx()
        [DllImport("user32.dll")]
        protected static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);
        // ************************************************************************
        #endregion

        #region �Զ������١��ṹ
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct HookEventMSG
        {
            //�˴�������int��
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;

        }
        #endregion
    }
}
