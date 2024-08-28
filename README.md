## Vehicle Physics Equation Description
These equations relate to the equations involving torque, power, force, drag, and rolling resistance.

## Engine Physics
### Engine Torque Equation
This is the max torque produced by the engine. Function of throttle and the max torque at a given RPM

```math
engineTorque = vehicleThrottle * maxTorque
```

### Horsepower Equation
Horsepower is a measurement of engine power

> convert torque in lb-ft and RPM to horsepower.
> horsePower equation comes from the definition of power in physics (P=τ⋅ω)
```math
P=τ⋅ω
```

```math
horsePower = engineTorque * currentEngineRPM / 5252 
```

### Drive torque to wheels

Torque transmissted to the drive wheels considering gear ratio and transmission efficiency.

```math
driveTorque = engineTorque * gearRatio * transmissionEfficiency
```

### Force at the Drive Wheels
This equation converts torque at the drive wheels into a linear force using the wheel radius.

Comes from the physics euqation relating torque to force

```math
τ=F⋅r
```
