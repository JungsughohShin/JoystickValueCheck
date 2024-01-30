using System;
using SharpDX.DirectInput;

namespace EnumDevicesApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            MainForJoystick();

        }

        static void MainForJoystick()
        {


            //  <1> 기기 검색
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;//디바이스 인스턴스를 넣음

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)//Guid.Empty는 인스턴스가 안들어 있는 것이다 .
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;// 다시 인스턴스 넣어라
            //게임패드 먼저 검색후, 조이 스틱 탐색

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick/Gamepad found.");
                Console.ReadKey();//?
                Environment.Exit(1);//아무거나 입력하면 강제 종료된다

            }
            // <2> 조이스틱 선언

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid); //조이스틱 객체 선언

            Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            //서보모터 2개만 사용
            int left_Y = 0; //모터 세기 (0~32768) >> ( 0~32)
            int left_X = -1; //안쓰는 값 
            int right_Y = -1; //안쓰는 값
            int right_X = 0;// (-32767 ~ 327678)>> ( -32 ~ 32) (-):좌회전, (+):우회전 
            int rightBtn_Up = 0; //1일때는 상승 동작, 0일 때는 동작 X
            int rightBtn_Dn = 0; //1일때는 하강 동작, 0일 때는 동작 X

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();//조이스틱 조작할 시에 
                /*var datas = joystick.GetBufferedData();
               

               

                foreach (var state in datas)
                    Console.WriteLine(state);
       
                */

                //                왼쪽
                //- X 왼쪽 0 오른쪽 65535(1000단위로 나눔)
                //- Y 아래 65535 , 위0
                //- Buttons8: 누를때 128, 땔때 0

                //오른쪽 조이스틱(날개 좌우 위아래)
                //-rotation x: 왼쪽 0 오른쪽 65535(1000단위로 나눔)
                //- rotation Y 아래 65535 , 위0
                //--Buttons9: 누를때 128, 땔때 0

                //오른쪽 7번 버튼 
                //누르고 있으면 128, 때는 순간 0
                //이름은 Button5

                //오른쪽 8번 버튼 
                //누르고 있으면 128, 때는 순간 0
                //이름은 Z



                JoystickUpdate[] states = joystick.GetBufferedData();

                foreach (var state in states)
                {
                    //Console.WriteLine(state);
                    
                    int val = state.Value;
                    //JoystickOffset offset = state.Offset;
                    JoystickOffset joyType = state.Offset;
                    String strType = joyType.ToString();

                    //Console.Write(strType);
                    //Console.WriteLine(val);

                   

                    if (strType == "Y")
                    {
                        int val2 = val - 32767;//원점을 0으로 잡기 위함이다. 
                        if (val2 < 0)
                        {
                            val2 = val2 * (-1);

                        }

                        left_Y = val2 / 1000;// (0~32768) >> (0~32)

                    }
                    else if (strType == "RotationX")
                    {
                        int val2 = val - 32767;//원점을 0으로 잡기 위함이다. 

                        if (val2 < 0)
                        {
                            val2 = val2 * (-1);
                            val2 = val2 / 1000;
                            right_X = val2 * (-1);


                        }
                        else if (val2 > 0)
                        {
                            right_X = val2 / 1000;
                            
                        }

                    }
                    else if (strType == "Buttons5")
                    {   if (val == 128)
                        {
                            rightBtn_Up = 1;
                        }
                        else {
                            rightBtn_Up = 0;

                        }
                       
                        

                    }
                    else if (strType == "Z")
                    {
                        if (val == 128)
                        {
                            rightBtn_Dn = 1;
                        }
                        else
                        {
                            rightBtn_Dn = 0;

                        }


                    }
                    Console.Write("Speed : ");
                    Console.Write(left_Y);
                    Console.Write("  ");

                    Console.Write("Turn : ");
                    Console.Write(right_X);
                    Console.Write("  ");

                    Console.Write("UP : ");
                    Console.Write(rightBtn_Up);
                    Console.Write("  ");

                    Console.Write("DOWN : ");
                    Console.Write(rightBtn_Dn);
                    Console.WriteLine("  ");

             

                }
            }



        }
    }
}