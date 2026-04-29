using System;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a robot unit controlled by a team and configured by the player.
    /// A robot is composed of four parts: brain, body, battery, and motor.
    /// Each part contributes to the robot's attributes.
    /// </summary>
    public class Robot
    {
        // Identity
        public Guid Id { get; set; }
        public Guid? TeamId { get; set; } // Nullable, robot can be without a team
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Planet { get; set; } = string.Empty;
        public int CreationDay { get; set; }
        public int CreationYear { get; set; }
        public string Face { get; set; } = string.Empty;

        // Component references
        public Guid BrainId { get; set; }
        public Guid BodyId { get; set; }
        public Guid MotorId { get; set; }
        public Guid BatteryId { get; set; }

        // Management attributes
        public int CurrentMentalShape { get; set; }
        public int CurrentCondition { get; set; }
        public int WearSpeed { get; set; }
        public int RecoveryRate { get; set; }
        public int Fatigue { get; set; }

        // Unavailability
        public int MatchesBannedRemaining { get; set; }
        public int MatchesUnavailableRemaining { get; set; }
    }

    /// <summary>
    /// Brain component: defines decision-making and behaviour attributes.
    /// </summary>
    public class RobotBrain
    {
        public Guid Id { get; set; }
        public BrainType BrainType { get; set; }
        public int ReactionTime { get; set; }
        public int ViewRange { get; set; }
        public int Anticipation { get; set; }
        public int ShootingAccuracy { get; set; }
    }

    public enum BrainType
    {
        Default,
        Attacker,
        Defender,
        Keeper,
        Playmaker,
        // Add more as needed
    }

    /// <summary>
    /// Body component: defines physical attributes.
    /// </summary>
    public class RobotBody
    {
        public Guid Id { get; set; }
        public int Mass { get; set; }
        public int Traction { get; set; }
        public int RotationResistance { get; set; }
        public int ShootingPower { get; set; }
    }

    /// <summary>
    /// Battery component: defines energy attributes.
    /// </summary>
    public class RobotBattery
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public int DischargeRate { get; set; }
        public int ConversionEfficiency { get; set; }
    }

    /// <summary>
    /// Motor component: defines movement and thermal attributes.
    /// </summary>
    public class RobotMotor
    {
        public Guid Id { get; set; }
        public int MaxSpeed { get; set; }
        public int Acceleration { get; set; }
        public int Braking { get; set; }
        public int MaxRotationSpeed { get; set; }
        public int RotationAcceleration { get; set; }
        public int HeatGenerationRate { get; set; }
        public int CoolingRate { get; set; }
        public int MaxTemperature { get; set; }
    }
}
