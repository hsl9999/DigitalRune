﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Globalization;


namespace DigitalRune.Particles
{
  partial class ParticleEffector
  {
    /// <summary>
    /// Creates a new <see cref="ParticleEffector"/> that is a clone of the current instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="ParticleEffector"/> that is a clone of the current instance.
    /// </returns>
    /// <remarks>
    /// <strong>Notes to Inheritors:</strong> The method <see cref="Clone"/> calls 
    /// <see cref="CreateInstanceCore"/> which is responsible for creating a new instance of the 
    /// <see cref="ParticleEffector"/> derived class and <see cref="CloneCore"/> to create a copy of
    /// the current instance. Classes that derive from <see cref="ParticleEffector"/> need to 
    /// implement <see cref="CreateInstanceCore"/> and <see cref="CloneCore"/>.
    /// </remarks>
    public ParticleEffector Clone()
    {
      var clone = CreateInstance();
      clone.CloneCore(this);
      return clone;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffector"/> class. 
    /// </summary>
    /// <returns>The new instance.</returns>
    /// <remarks>
    /// This is a protected method, and the actual object-specific implementations for the behavior 
    /// are dependent on the override implementation of the <see cref="CreateInstanceCore"/> method, 
    /// which this method calls internally. 
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Cannot clone particle effector. A derived class does not implement 
    /// <see cref="CreateInstanceCore"/>.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateInstanceCore")]
    private ParticleEffector CreateInstance()
    {
      ParticleEffector newInstance = CreateInstanceCore();
      if (GetType() != newInstance.GetType())
      {
        string message = String.Format(
          CultureInfo.InvariantCulture,
          "Cannot clone particle effector. The derived class {0} does not implement CreateInstanceCore().",
          GetType());

        throw new InvalidOperationException(message);
      }

      return newInstance;
    }


    /// <summary>
    /// When implemented in a derived class, creates a new instance of the 
    /// <see cref="ParticleEffector"/> derived class. 
    /// </summary>
    /// <returns>The new instance.</returns>
    /// <remarks>
    /// <para>
    /// Do not call this method directly (except when calling base in an implementation). This 
    /// method is called internally by the <see cref="Clone"/> method whenever a new instance of the
    /// <see cref="ParticleEffector"/> is created. 
    /// </para>
    /// <para>
    /// <strong>Notes to Inheritors:</strong> Every <see cref="ParticleEffector"/> derived class 
    /// must implement this method. A typical implementation is to simply call the default 
    /// constructor and return the result. 
    /// </para>
    /// </remarks>
    protected abstract ParticleEffector CreateInstanceCore();


    /// <summary>
    /// Makes the instance a clone of the specified <see cref="ParticleEffector"/>.
    /// </summary>
    /// <param name="source">The object to clone.</param>
    /// <remarks>
    /// <strong>Notes to Inheritors:</strong> Every <see cref="ParticleEffector"/> derived class 
    /// must implement this method. A typical implementation is to call <c>base.CloneCore(this)</c> 
    /// to copy all properties of the base class and then copy all properties of the derived class.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    protected virtual void CloneCore(ParticleEffector source)
    {
      Enabled = source.Enabled;
      Name = source.Name;
    }
  }
}
