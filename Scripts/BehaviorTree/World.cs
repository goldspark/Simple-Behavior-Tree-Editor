using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{
    /// <summary>
    /// Main AI loop. Must be put in a game worldd in order for Behavior Trees of NPCs to work!
    /// Create new AI class as a derive of this one
    /// </summary>
    public abstract class World
    {
        /// <summary>
        /// Collection of active npcs in the world
        /// </summary>
        public static List<GoldTreeBase> npcs = new List<GoldTreeBase>();

        
        private Stopwatch stopwatch;
        //You may edit TargetFPS value appropriate for your project

        /// <summary>
        /// Set FPS for ai in function UpdateAIMT().
        /// It will override function argument if it is greater than 0.
        /// </summary>
        public static double FPS = 0;





        /// <summary>
        /// General AI Update loop for multi threading.
        /// You may call this from other thread simply by doing: <code>Task.Run(() -> UpdateAIMT());</code>
        /// Be sure to check if an npc instance is valid correctly
        /// by implementing CheckValid() function correctly.
        /// <example>
        /// In Godot for example:
        /// <code>
        /// bool CheckValid(GoldTreeBase npc){
        ///     return IsInstanceValid(npc);
        /// }
        /// </code>
        /// </example>
        /// <paramref name="fps"/>: Set frames per second on which this loop will run at. If 0 it will run at 30 fps
        /// </summary>
        public virtual void UpdateAIMT(double fps)
        {

            double TargetFPS;

            if (FPS > 0)
            {
                TargetFPS = FPS;
            }
            else if (fps == 0)
                TargetFPS = 30.0f;
            else
            {
                TargetFPS = fps;
            }

            double TargetFrameTime = 1.0 / Math.Abs(TargetFPS);

            stopwatch = Stopwatch.StartNew();

            while (true)
            {
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();


                for (int i = 0; i < npcs.Count; i++)
                {

                    if (CheckValid(npcs[i]))
                    {
                        npcs[i].Update();
                    }
                    else
                    {
                        npcs.RemoveAt(i);
                    }

                }


                double frameTime = stopwatch.Elapsed.TotalSeconds;
                double sleepTime = TargetFrameTime - frameTime;

                if (sleepTime > 0)
                {
                    System.Threading.Thread.Sleep((int)(sleepTime * 1000));
                }

            }

        }

        /// <summary>
        /// General AI Update loop that does not contain fps loop like UpdateAIMT.
        /// Be sure to check if an npc instance is valid correctly
        /// by implementing CheckValid() function correctly.
        /// <example>
        /// In Godot for example:
        /// <code>
        /// bool CheckValid(GoldTreeBase npc){
        ///     return IsInstanceValid(npc);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public virtual void UpdateAI()
        {
            for (int i = 0; i < npcs.Count; i++)
            {

                if (CheckValid(npcs[i]))
                {
                    npcs[i].Update();
                }
                else
                {
                    npcs.RemoveAt(i);
                }

            }
        }

        public abstract bool CheckValid(GoldTreeBase node);
        

    }


}
