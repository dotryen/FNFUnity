using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SideColor : int { Purple, Blue, Green, Red }

public enum SideDir : int { Left, Down, Up, Right }

public enum NoteState { TooEarly, Good, TooLate }

public enum NoteResult { None, Hit, Miss }

public enum NoteType : int { Standard, Sustain, SustainEnd }
