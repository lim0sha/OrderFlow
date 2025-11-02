UPDATE order_items
SET
    order_id = @order_id,
    product_id = @product_id,
    order_item_quantity = @order_item_quantity,
    order_item_deleted = @order_item_deleted
WHERE order_item_id = @order_item_id;