using Nintaco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nintaco;

namespace BombermanBot
{
    class Program
    {
        private readonly RemoteAPI api = ApiSource.API;

        public void launch()
        {
            api.AddFrameListener(renderFinished);
            api.AddStatusListener(statusChanged);
            api.AddActivateListener(apiEnabled);
            api.AddDeactivateListener(apiDisabled);
            api.AddStopListener(dispose);
            api.Run();
        }

        private void apiEnabled()
        {
            Console.WriteLine("API enabled");
            //api.reset(); // Uncomment this to reset the console
            stateStartFrame = api.GetFrameCount();
        }

        private void apiDisabled()
        {
            Console.WriteLine("API disabled");
        }

        private void dispose()
        {
            Console.WriteLine("API stopped");
        }

        private void statusChanged(String message)
        {
            Console.WriteLine("Status message: {0}", message);
        }

        int state = 0;
        // 0 - go right
        // 1 - drop bomb
        // 2 - go left
        // 3 - wait

        private int stateStartFrame = 0;

        readonly int spaceTraversalTime = 20;

        private void renderFinished()
        {
            int currentFrame = api.GetFrameCount();


            switch (state)
            {
                case 0:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.Right, true);
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 4)
                    {
                        state = 1;
                        stateStartFrame = currentFrame;
                    }
                    break;
                case 1:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.A, true);
                    state = 2;
                    stateStartFrame = currentFrame;
                    break;
                case 2:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.Left, true);
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 3)
                    {
                        state = 3;
                        stateStartFrame = currentFrame;
                    }
                    break;
                case 3:
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 6)
                    {
                        state = 0;
                        stateStartFrame = currentFrame;
                    }
                    break;
            }


            //for(int i = 8386; i < 9436; i+= 2)
            //{
            //    var block = api.readPPU(i);
            //    if (block == 95) Console.Write("O");
            //    else if (block == 104) Console.Write("X");
            //    else Console.Write("-");
            //}


            // 15F is green
            // 168 is brick
            // 166 is block
            //Console.WriteLine(api.readPPU(8386));
            //Console.WriteLine(api.readPPU(8394));
            //Console.WriteLine(api.getFrameCount());

        }
        static void Main(string[] args)
        {
            ApiSource.initRemoteAPI("localhost", 9999);

            new Program().launch();
        }
    }
}
