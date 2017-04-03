using CitizenFX.Core;
using System;

namespace Derby
{
    class Derby : BaseScript
    {
        private bool alreadySpawned;

        public Derby()
        {
            EventHandlers["derby:setspawn"] += new Action<float, float, float, float, float, float, float>((x, y, z, heading, spectateX, spectateY, spectateZ) =>
            {
                DerbyGame.SetSpawn(new Vector3(x, y, z), heading, new Vector3(spectateX, spectateY, spectateZ));
            });
            EventHandlers["derby:startgame"] += new Action(delegate
            {
                DerbyGame.NewDerbyGame();
            });

            EventHandlers["playerSpawned"] += new Action<dynamic>(stuff =>
            {
                if (!alreadySpawned)
                {
                    TriggerServerEvent("derby:newplayer", Game.Player.ServerId);
                    alreadySpawned = true;
                }
            });

            if (Game.PlayerPed != null) Game.PlayerPed.HealthFloat = 0f;
        }
    }
}
