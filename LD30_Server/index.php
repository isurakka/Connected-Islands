<?php
	define ("DB_HOST", "127.0.0.1");
	define ("DB_NAME", "LD30");
	define ("DB_USER", "LD30");
	define ("DB_PASS", "");
	define ("NetSeparator", "bJqjhyNsvA3wffogBGL5qpxoQ3mNemK7");
	define ("MessageRequest", "i5tbjtHCXa0fGCvZW98wrrWzAHPRRw88");

	if (isset($_POST))
	{
		if ($_POST["id"] === "-1")
		{
			$db = new PDO("mysql:host=" . DB_HOST . ";dbname=" . DB_NAME, DB_USER, DB_PASS);
			$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

			$st = $db->prepare("
			INSERT INTO message (
				id,
				message,
				regards,
				time,
				views)
			VALUE (
				NULL,
				:message,
				:regards,
				:time,
				:views)
			");

			$message = $_POST["message"];
			$regards = $_POST["regards"];
			$views = "1";

			$st->bindParam(":message", $message);
			$st->bindParam(":regards", $regards);
			$st->bindParam(":time", time());
			$st->bindParam(":views", $views);
			$st->execute();

			echo "OK";
		}
		else if (isset($_POST["MessageRequest"]) && $_POST["MessageRequest"] == MessageRequest)
		{
			$db = new PDO("mysql:host=" . DB_HOST . ";dbname=" . DB_NAME, DB_USER, DB_PASS);
			$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
			$query = "
				SELECT *
				FROM message
				WHERE time < UNIX_TIMESTAMP() - 600
				ORDER BY views ASC, time DESC
				LIMIT 40
				";
			$prepare = $db->prepare($query);
			$prepare->execute();

			$prepare->setFetchMode(PDO::FETCH_ASSOC);
			$rows = array();
			while ($row = $prepare->fetch())
			{
				$rows[] = $row;
			}
			$key = array_rand($rows);
			echo $rows[$key]["id"] . NetSeparator . $rows[$key]["message"] . NetSeparator . $rows[$key]["regards"] . NetSeparator . $rows[$key]["time"] . NetSeparator . $rows[$key]["views"];
		}
	}
?>