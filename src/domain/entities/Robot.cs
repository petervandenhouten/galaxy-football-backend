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
        public DateTime CreationDate { get; set; }
        public string Face { get; set; } = string.Empty;

        // Component references
        public Guid BrainId { get; set; }
        public Guid BodyId { get; set; }
        public Guid MotorId { get; set; }
        public Guid BatteryId { get; set; }

        // Management attributes
        public double CurrentMentalShape { get; set; } 
        public double CurrentCondition { get; set; } 
        public double WearSpeed { get; set; }
        public double RecoveryRate { get; set; }
        public double Fatigue { get; set; }

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
        public double ReactionTime { get; set; }
        public double ViewRange { get; set; }
        public double Anticipation { get; set; }
        public double ShootingAccuracy { get; set; }
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
        public double Mass { get; set; }
        public double Traction { get; set; }
        public double RotationResistance { get; set; }
        public double ShootingPower { get; set; }
    }

    /// <summary>
    /// Battery component: defines energy attributes.
    /// </summary>
    public class RobotBattery
    {
        public Guid Id { get; set; }
        public double Capacity { get; set; }
        public double DischargeRate { get; set; }
        public double ConversionEfficiency { get; set; }
    }

    /// <summary>
    /// Motor component: defines movement and thermal attributes.
    /// </summary>
    public class RobotMotor
    {
        public Guid Id { get; set; }
        public double MaxSpeed { get; set; }
        public double Acceleration { get; set; }
        public double Braking { get; set; }
        public double MaxRotationSpeed { get; set; }
        public double RotationAcceleration { get; set; }
        public double HeatGenerationRate { get; set; }
        public double CoolingRate { get; set; }
        public double MaxTemperature { get; set; }
    }
}
