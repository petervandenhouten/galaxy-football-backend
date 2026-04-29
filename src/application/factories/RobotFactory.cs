using System;
using GalaxyFootball.Domain.Entities;
using GalaxyFootball.Application.Utils;

namespace GalaxyFootball.Application.Factories
{
    public static class RobotFactory
    {
        private static readonly Random _random = new Random();


        public static (Robot robot, RobotBrain brain, RobotBody body, RobotBattery battery, RobotMotor motor) CreateRobot(int year, int day, Guid? teamId = null)
        {
            var nameGen = new NameGenerator();

            var brain = new RobotBrain
            {
                Id = Guid.NewGuid(),
                BrainType = GetRandomEnum<BrainType>(),
                ReactionTime = RandomInt(),
                ViewRange = RandomInt(),
                Anticipation = RandomInt(),
                ShootingAccuracy = RandomInt()
            };
            var body = new RobotBody
            {
                Id = Guid.NewGuid(),
                Mass = RandomInt(),
                Traction = RandomInt(),
                RotationResistance = RandomInt(),
                ShootingPower = RandomInt()
            };
            var battery = new RobotBattery
            {
                Id = Guid.NewGuid(),
                Capacity = RandomInt(),
                DischargeRate = RandomInt(),
                ConversionEfficiency = RandomInt()
            };
            var motor = new RobotMotor
            {
                Id = Guid.NewGuid(),
                MaxSpeed = RandomInt(),
                Acceleration = RandomInt(),
                Braking = RandomInt(),
                MaxRotationSpeed = RandomInt(),
                RotationAcceleration = RandomInt(),
                HeatGenerationRate = RandomInt(),
                CoolingRate = RandomInt(),
                MaxTemperature = RandomInt()
            };
            var robot = new Robot
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                FirstName = nameGen.GetFirstName(),
                LastName = nameGen.GetLastName(),
                Planet = nameGen.GetPlanetName(),
                CreationYear = year,
                CreationDay = day,
                Face = $"face_{_random.Next(1, 1000)}.png",
                BrainId = brain.Id,
                BodyId = body.Id,
                MotorId = motor.Id,
                BatteryId = battery.Id,
                CurrentMentalShape = RandomInt(),
                CurrentCondition = RandomInt(),
                WearSpeed = RandomInt(),
                RecoveryRate = RandomInt(),
                Fatigue = RandomInt(),
                MatchesBannedRemaining = 0,
                MatchesUnavailableRemaining = 0
            };
            return (robot, brain, body, battery, motor);
        }

        private static int RandomInt()
        {
            return _random.Next(0, 100);
        }

        private static TEnum GetRandomEnum<TEnum>() where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum));
            return (TEnum)values.GetValue(_random.Next(values.Length));
        }
    }
}
