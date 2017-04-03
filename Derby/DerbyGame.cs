using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace Derby
{
    public class DerbyGame : BaseScript
    {
        private static float DERBY_PLAYEROUT_HEALTHTHRESHOLD = 900f;

        private static bool isPlayerOut = true;
        private static Vector3 spawn = new Vector3();
        private static float spawnHeading;
        private static Vector3 spectatePos = new Vector3();

        public DerbyGame()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            if (IsPlayerInRound(Game.Player))
            {
                Ped playerPed = Game.PlayerPed;
                playerPed.IsInvincible = true;

                DisableControls();
                CheckPlayerVehTotalled();
                UpdatePlayersHealthDisplay();
            }
            else
            {
                RemovePlayersHealthDisplay();
            }

            await Task.FromResult(0);
        }

        private void DisableControls()
        {
            Game.DisableControlThisFrame(1, Control.VehicleHandbrake);
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.VehicleAttack);
        }

        public static void SetSpawn(Vector3 newSpawn, float heading, Vector3 pSpectatePos)
        {
            spawn = newSpawn;
            spawnHeading = heading;
            spectatePos = pSpectatePos;
        }

        public static async void NewDerbyGame()
        {
            await PreGame();

            Vehicle veh = await SpawnDerbyVeh();
            PreparePlayer(veh);
            await StartCountdown(veh);
        }

        private static async Task PreGame()
        {
            Ped playerPed = Game.PlayerPed;
            if (IsPlayerInRound(Game.Player))
            {
                isPlayerOut = true;
                playerPed.CurrentVehicle.Delete();
                playerPed.IsPositionFrozen = true;
            }
            await Delay(7000);
        }

        private static async Task<Vehicle> SpawnDerbyVeh()
        {
            Vehicle veh = await World.CreateVehicle(VehicleHash.Asterope, spawn);
            Util.SetVehNumPlate(veh, "DERBY");
            int r, g, b;
            Util.GetPlayerRGBColor(out r, out g, out b);
            Util.SetVehRGBColor(veh, r, g, b);

            veh.Heading = spawnHeading;
            veh.LockStatus = VehicleLockStatus.StickPlayerInside;
            veh.IsExplosionProof = true;
            veh.IsEngineRunning = true;

            veh.PlaceOnGround();

            return veh;
        }

        private static void PreparePlayer(Vehicle derbyVeh)
        {
            Ped playerPed = Game.PlayerPed;
            playerPed.IsCollisionEnabled = true;
            playerPed.IsVisible = true;

            playerPed.SetIntoVehicle(derbyVeh, VehicleSeat.Driver);
            derbyVeh.MarkAsNoLongerNeeded();

            isPlayerOut = false;
        }

        private bool IsEntityInArea(Entity entity, Vector3 boundry1, Vector3 boundry2)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_AREA, entity.Handle, boundry1.X, boundry1.Y, boundry1.Z, boundry2.X, boundry2.Y, boundry2.Z, true, true, true);
        }

        private void UpdateHealthDisplay(Ped ped, float health)
        {
            string displayHealth = $"{Math.Ceiling(health - DERBY_PLAYEROUT_HEALTHTHRESHOLD)}%";
            if (health - DERBY_PLAYEROUT_HEALTHTHRESHOLD <= 0)
            {
                displayHealth = "OUT!";
            }

            int headId = Function.Call<int>(Hash._CREATE_HEAD_DISPLAY, ped.Handle, "", false, false, "", true);
            Function.Call(Hash._SET_HEAD_DISPLAY_STRING, headId, displayHealth); // TODO: Make this a actual percent display
            Function.Call((Hash)0xD48FE545CD46F857, headId, 0, 200); // Alpha
            Function.Call((Hash)0x613ED644950626AE, headId, 0, Convert.ToInt32(health) / 50); // Color
        }

        private void RemoveHealthDisplay(Ped ped)
        {
            int headId = Function.Call<int>(Hash._CREATE_HEAD_DISPLAY, ped.Handle, "", false, false, "", true);
            Function.Call((Hash)0x31698AA80E0223F8, headId); // Destroy head display
        }

        private void CheckPlayerVehTotalled()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh.HealthFloat <= DERBY_PLAYEROUT_HEALTHTHRESHOLD || veh.IsInWater) SetPlayerOut(veh);
        }

        private void UpdatePlayersHealthDisplay()
        {
            foreach (Player player in Players)
            {
                if (IsPlayerInRound(player))
                {
                    Vehicle playerVeh = player.Character.CurrentVehicle;
                    UpdateHealthDisplay(player.Character, playerVeh.HealthFloat);
                }
            }
        }

        private void RemovePlayersHealthDisplay()
        {
            foreach (Player player in Players)
            {
                RemoveHealthDisplay(player.Character);
            }
        }

        private static async Task StartCountdown(Vehicle veh)
        {
            Game.Player.CanControlCharacter = false;

            for (int i = 3; i > -1; i--)
            {
                if (i == 0)
                {
                    Game.Player.CanControlCharacter = true;
                    TriggerEvent("derby:showcountdown", "GO!");
                }
                else TriggerEvent("derby:showcountdown", i);

                await Delay(1000);
            }
            TriggerEvent("derby:showcountdown", -1);
        }

        private async void SetPlayerOut(Vehicle veh)
        {
            isPlayerOut = true;
            TriggerServerEvent("derby:playerout", Game.Player.ServerId);

            RemovePlayersHealthDisplay();

            Ped playerPed = Game.PlayerPed;
            playerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut); // Makes player not leave vehicle
            World.AddExplosion(veh.Position, ExplosionType.Car, 1f, 0f);

            await ShowOutScreenEffect();

            veh.Delete();
            SetPlayerSpectator();
        }

        private async Task ShowOutScreenEffect()
        {
            Ped playerPed = Game.PlayerPed;
            Screen.Effects.Start(ScreenEffect.DeathFailMpIn, 5000);
            Game.PlaySound("Bed", "WastedSounds");
            await Delay(5000);
        }

        private void SetPlayerSpectator()
        {
            Ped playerPed = Game.PlayerPed;
            playerPed.Position = spectatePos;
            playerPed.IsPositionFrozen = true;
            playerPed.IsCollisionEnabled = false;
            playerPed.IsVisible = false;
            Game.Player.CanControlCharacter = false;
        }

        private static bool IsPlayerInRound(Player player)
        {
            if (player == Game.Player) return player.Character != null && !isPlayerOut;
            else
            {
                return player.Character != null
                    && player.Character.CurrentVehicle != null
                    && player.Character.CurrentVehicle.HealthFloat > DERBY_PLAYEROUT_HEALTHTHRESHOLD;
            }
        }
    }
}
