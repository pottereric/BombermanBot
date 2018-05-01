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
        private const int MoveRight = 0;
        private const int DropABomb = 1;
        private const int MoveLeft = 2;
        private const int Wait = 3;

        private int stateStartFrame = 0;

        readonly int spaceTraversalTime = 20;

        private void renderFinished()
        {
            int currentFrame = api.GetFrameCount();


            switch (state)
            {
                case MoveRight:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.Right, true);
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 4)
                    {
                        state = DropABomb;
                        stateStartFrame = currentFrame;
                    }
                    break;
                case DropABomb:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.A, true);
                    state = MoveLeft;
                    stateStartFrame = currentFrame;
                    break;
                case MoveLeft:
                    ApiSource.API.WriteGamepad(0, GamepadButtons.Left, true);
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 3)
                    {
                        state = Wait;
                        stateStartFrame = currentFrame;
                    }
                    break;
                case Wait:
                    if (currentFrame - stateStartFrame > spaceTraversalTime * 6)
                    {
                        state = MoveRight;
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
