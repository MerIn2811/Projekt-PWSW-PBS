<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

header('Content-Type: application/json');

$method = $_SERVER['REQUEST_METHOD'];
if ($method !== 'PATCH') {
    http_response_code(405);
    echo json_encode(['message' => 'Method not allowed']);
    exit;
}
$token = get_bearer_token();

$raw = file_get_contents('php://input');
$body = json_decode($raw, true);

if (!is_array($body)) {
    http_response_code(400);
    echo json_encode(['message' => 'Body musi być JSON-em']);
    exit;
}

// allowlista pól do update
$allowed = ['name', 'description', 'isFinished', 'endDate', 'category'];

$data = [];
foreach ($body as $k => $v) {
    if (in_array($k, $allowed, true)) {
        $data[$k] = $v;
    }
}

if (count($data) === 0) {
    http_response_code(400);
    echo json_encode(['message' => 'Brak pól do aktualizacji']);
    exit;
}


$goalId = $body['goalId'] ?? null;

if (!$goalId || !ctype_digit((string)$goalId)) {
    http_response_code(400);
    echo json_encode(['message' => 'Nieprawidłowe ID 1']);
    exit;
}



$setParts = [];
$params = [];
foreach ($data as $col => $val) {
    $setParts[] = "`$col` = ?";
    $params[] = $val;
}
$params[] = (int)$goalId;

$sql = "UPDATE goal SET " . implode(', ', $setParts) . " WHERE idGoal = ?";
$stmt = $pdo->prepare($sql);
$stmt->execute($params);

if ($stmt->rowCount() === 0) {
    $chk = $pdo->prepare("SELECT idGoal FROM goal WHERE idGoal = ?");
    $chk->execute([(int)$goalId]);
    if (!$chk->fetchColumn()) {
        http_response_code(404);
        echo json_encode(['message' => 'Goal nie istnieje']);
        exit;
    }
}

echo json_encode(['message' => 'OK', 'updated_fields' => array_keys($data)]);

?>