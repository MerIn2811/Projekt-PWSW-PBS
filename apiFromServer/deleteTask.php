<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

$userId = require_auth($pdo);

$taskId = isset($_GET['taskId']) ? (int)$_GET['taskId'] : 0;
if ($taskId <= 0) {
    $raw = file_get_contents("php://input");
    $data = json_decode($raw, true);

    if (is_array($data) && isset($data['taskId'])) {
        $taskId = (int)$data['taskId'];
    }
}

if ($taskId <= 0) respond(400, ["error" => "Missing taskId"]);


$stmt = $pdo->prepare("
    DELETE FROM task WHERE idTask = :tid
");
$stmt->execute(["tid" => $taskId]);

if ($stmt->rowCount() === 1) respond(200, ["task" => "ok!"]);
else respond(406, ["task" => "task not deleted"]);

?>