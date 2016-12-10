# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0010_auto_20150429_1041'),
    ]

    operations = [
        migrations.RenameField(
            model_name='api',
            old_name='pub_date',
            new_name='edit_date',
        ),
    ]
