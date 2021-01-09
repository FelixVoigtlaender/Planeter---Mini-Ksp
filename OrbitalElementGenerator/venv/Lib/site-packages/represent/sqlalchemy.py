# code: utf-8
from __future__ import absolute_import, print_function

try:
    import sqlalchemy as sa
except ImportError:
    sa = None

from .core import ReprHelperMixin


class ModelReprMixin(ReprHelperMixin):
    def _repr_helper_(self, r):
        inspector = sa.inspect(self)
        for col in inspector.mapper.column_attrs:
            attr_state = getattr(inspector.attrs, col.key)

            # This is a new state in SQLAlchemy v1.1
            try:
                deleted = inspector.deleted
            except AttributeError:
                deleted = False

            # Determine if the instance came from or represents a row in the
            # database.
            from_db = inspector.persistent or inspector.detached or deleted

            # Don't try to load unloaded values
            if from_db and attr_state.loaded_value is sa.orm.base.NO_VALUE:
                r.keyword_with_value(col.key, '<not loaded>', raw=True)
            else:
                r.keyword_from_attr(col.key)
