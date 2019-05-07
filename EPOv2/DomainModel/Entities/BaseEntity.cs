namespace DomainModel.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class BaseEntity<T>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [Key]
        public T Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when record was originally created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the record's last modified date and time.
        /// </summary>
        [Display(Name = "Modified date")]
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the username of a user who originally created entity.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the username of a user who last modified record.
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Display(Name = "Modified by")]
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether record is soft deleted (marked as inactive).
        /// </summary>
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the time stamp for concurrency check.
        /// </summary>
        [Timestamp]
        public byte[] TimeStamp { get; set; }
    }

    /// <summary>
    /// The base entity class that contains common fields.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class BaseEntity : BaseEntity<int>
    {
    }
}