

import json

def write_json(data, filename):
    with open(filename, "w") as f:
        json.dump(data,f,indent=4)

def generatePlanet(name, center, a_semiMajorAxis, e_eccentricity, mass, radius):
    print(f'{name}: Center:{center} semiMajorAxis: {a_semiMajorAxis}, eccentricity: {e_eccentricity}, mass: {mass}, radius {radius}')

    #Data to be written
    orbitElement = {
        "name" : name,
        "center" : center,
        "a_semiMajorAxis" : a_semiMajorAxis,
        "e_eccentricity" : e_eccentricity,
        "mass" : mass,
        "radius" : radius
    }
    return orbitElement


#Serialize Json
planets = {
    "Planets" : {

    }
}
temp = planets["Planets"]


#Planets
earth = generatePlanet("Earth","Sun",1.000003,0.01671,1.0000,1.0000)
temp.append(earth)
temp.append(earth)
temp.append(earth)

write_json(temp,"Planets.json")

