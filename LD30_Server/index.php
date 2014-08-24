<?php
	define ("DB_HOST", "127.0.0.1");
	define ("DB_NAME", "LD30");
	define ("DB_USER", "LD30");
	define ("DB_PASS", "");
	define ("NetSeparator", "bJqjhyNsvA3wffogBGL5qpxoQ3mNemK7");
	define ("MessageRequest", "i5tbjtHCXa0fGCvZW98wrrWzAHPRRw88");
	define ("GuestbookRequest", "CQrxp2zpbWUNRDYaLeoLOpQait2rHk2N");

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
		else if (isset($_POST["GuestbookRequest"]) && $_POST["GuestbookRequest"] == GuestbookRequest && isset($_POST["where"]))
		{
			$db = new PDO("mysql:host=" . DB_HOST . ";dbname=" . DB_NAME, DB_USER, DB_PASS);
			$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
			$st = $db->prepare("SELECT * FROM guestbook WHERE whereis = :where ORDER BY time DESC");
			$where = $_POST["where"];

			$st->bindParam(":where", $where);
			$st->execute();

			$st->setFetchMode(PDO::FETCH_ASSOC);
			$rows = array();
			$result = "";
			while ($row = $st->fetch())
			{
				$result = $result . $row["name"] . NetSeparator . $row["time"]. NetSeparator;
			}
			echo $result;
		}
		else if (isset($_POST["name"]) && isset($_POST["where"]))
		{
			$db = new PDO("mysql:host=" . DB_HOST . ";dbname=" . DB_NAME, DB_USER, DB_PASS);
			$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

			$st = $db->prepare("
			INSERT INTO guestbook (
				id,
				name,
				whereis,
				time)
			VALUE (
				NULL,
				:name,
				:where,
				:time)
			");

			$name = $_POST["name"];
			$where = $_POST["where"];

			$st->bindParam(":name", $name);
			$st->bindParam(":where", $where);
			$st->bindParam(":time", time());
			$st->execute();

			echo "OK";
		}
	}
?>