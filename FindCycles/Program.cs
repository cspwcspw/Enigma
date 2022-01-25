// See https://aka.ms/new-console-template for more information
using Utils;


Enigma mc = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());

CycleFinder bombe = new CycleFinder(mc);