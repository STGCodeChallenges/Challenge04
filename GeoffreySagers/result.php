<!doctype html>
<html>
<head>
<meta charset="utf-8">
<title>Order of Operations</title>
<link href="css/styles.css" rel="stylesheet" type="text/css">
<?php include('functions.php'); ?>
</head>
<body>
	<div class="results">
		<?php
			$equation = $_POST['equation'];
			echo($equation . " = ");
			echo(solveEquation($equation));
		?>
	</div>
	<div class="button"><a href="index.php">Solve another equation &raquo;</a></div>
</body>
</html>