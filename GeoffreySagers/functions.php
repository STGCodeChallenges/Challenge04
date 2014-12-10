<?php
function solveEquation($equation){
	$equation = str_replace(")(",")*(",$equation);
	$answer = create_function("", "return (" . $equation . ");" );
	return (0 + $answer());
	}
?>