// Aldrik Ramaekers 19/1/2019

// 4212FEC menu state
// 421EA98, 1 = zombies menu/game, 0 otherwise
// 2E8C8AC, 18568 = loading map
// 01809A34, ingame timer
// 2F08A30, ingame timer
// 2EE7F2C, 0 = paused, 1065353216 = playing
// 8902B4, 4 = paused, 0 = playing
// 165695D, 255 = level change, then 0 before level appears, then 255 again and then 0 when new round starts and round number color = red
// 168E564, can move (not good)
// 25A31CC , 25A4C9C, 25A66F4, 3570276 can move
// 1809A34, can move 
// 16569B6, level??

// current problems:
// 

state("BlackOps")
{
	int scene_state : 0x896268;
	int game_paused : 0x8902B4;
	int timer : 0x2F08A30;
	int menu_state : 0x4212FEC;
	int magic_level : 0x232DFDA;
	byte dead : 0x1808D34;
}

startup
{
    vars.timerModel = new TimerModel { CurrentState = timer };
	vars.is_paused = (vars.timerModel.CurrentState.CurrentPhase == TimerPhase.Paused);
	vars.did_reset = false;
	vars.timer_started = false;
	vars.timer_value = 0;
	vars.timer_pause_length = 0;
	vars.current_level = 1;
	vars.last_level_split = 1;
	vars.start_level = 1;
	vars.skipped_wonky_level = false;
	vars.level_change_stamp = 0;
	vars.level_tick = 0;
	
	vars.start_times = new List<Tuple<int, int>>
	{
      new Tuple<int, int>(5, 190),		// kino
      new Tuple<int, int>(10, 190),		// five
      new Tuple<int, int>(15, 50),		// dead ops
	  new Tuple<int, int>(20, 580),		// ascension
	  new Tuple<int, int>(52, 195),		// moon
	  new Tuple<int, int>(84, 195),		// nacht der untoten  
	  new Tuple<int, int>(116, 200),	// verruckt
	  new Tuple<int, int>(148, 198),	// shi no numa
	  new Tuple<int, int>(180, 195),	// der riese
	  
	  // call of the dead and shangri la not done yet
	};
}

start
{
	if (current.menu_state != 0) return false;
	
	foreach(Tuple<int, int> item in vars.start_times)
	{
		if (current.scene_state == item.Item1)
		{
			if (vars.timer_value > item.Item2) 
			{
				vars.timer_started = true;
				vars.current_level = current.magic_level;
				vars.start_level = current.magic_level;
				vars.last_level_split = current.magic_level;
				vars.skipped_wonky_level = false;
				return true;
			}
		}
	}
	
	return false;
}

/*
split
{
	if (vars.timer_started)
	{
		if (vars.current_level != vars.last_level_split)
		{
			int diff = vars.timer_value - vars.level_change_stamp;
			
			// wait untill level appears on screen
			if (diff > 60)
			{
				print(vars.current_level.ToString() + " --- " + vars.last_level_split.ToString());
				vars.last_level_split = vars.current_level;
				
				int lvl_to_check = vars.current_level;
				
				if (lvl_to_check <= 5 || lvl_to_check == 15 || (lvl_to_check % 10 == 0 && lvl_to_check <= 100))
				return true;
			}
		}
	}
	
	return false;
}
*/

reset
{
	if (!vars.did_reset && current.timer < 100)
	{
		vars.timer_started = false;
		vars.did_reset = true;
		vars.timer_value = 0;
		vars.timer_pause_length = 0;
		vars.current_level = 1;
		vars.last_level_split = 1;
		return true;
	}
	if (current.timer >= 100)
		vars.did_reset = false;
	return false;
}

update
{	
	if (current.game_paused == 4 && !vars.is_paused) {
		vars.is_paused = true;
			
		if (vars.timer_started)
			vars.timerModel.Pause();
	}
	else if (current.game_paused != 4 && vars.is_paused)
	{
			vars.is_paused = false;
			
		if (vars.timer_started)
			vars.timerModel.Pause();
	}
	
	
	if (old.dead != 0 && current.dead == 0)
	{
		if (vars.timer_started)
			vars.timerModel.Pause();
		vars.timer_started = false;
	}
	
	if (current.magic_level != old.magic_level)
	{
		vars.level_tick++;
		if (vars.level_tick % 2 == 1)
		{
			vars.current_level++;
			vars.level_change_stamp = vars.timer_value;
		}
	}
	
	// we have our own timer that is basically the ingame timer that pauses when the player pauses the game
	// we do this so the player can pause the game when spawning in and the timer hasnt started yet.
	if (!vars.is_paused)
	{
		vars.timer_value = current.timer - vars.timer_pause_length;
	}
	else
	{
		vars.timer_pause_length = current.timer - vars.timer_value;
	}
	
	//print(vars.timer_value.ToString());
	
	return true;
}