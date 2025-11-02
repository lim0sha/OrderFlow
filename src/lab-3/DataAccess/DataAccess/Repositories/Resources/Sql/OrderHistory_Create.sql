INSERT INTO order_history (order_id,
                           order_history_item_created_at,
                           order_history_item_kind,
                           order_history_item_payload)
VALUES (@order_id,
        @order_history_item_created_at,
        @order_history_item_kind,
        @order_history_item_payload::jsonb) RETURNING order_history_item_id;