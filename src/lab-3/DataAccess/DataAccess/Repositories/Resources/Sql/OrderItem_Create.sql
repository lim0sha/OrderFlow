INSERT INTO order_items (order_id,
                         product_id,
                         order_item_quantity,
                         order_item_deleted)
VALUES (@order_id,
        @product_id,
        @order_item_quantity,
        @order_item_deleted) RETURNING order_item_id;